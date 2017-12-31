/*
 * BeatDetector.c
 *
 * Created: 2013-05-25 15:58:23
 *  Author: pbejger
 */ 

#include "BeatDetector.h"

void InitSoundEnergyData(SoundEnergyData_t *data, double *historyBuffer)
{
    data->History = historyBuffer;
    ResetSoundEnergyData(data);
}

void ResetSoundEnergyData(SoundEnergyData_t *data)
{
    data->CurrentValue = 0;
    data->SampleIndex = 0;
    data->NewSetAwait = FALSE;
}

void InitBeatDetectorConfiguration(BeatDetectorConfiguration_t *configuration)
{
    configuration->FuFreqBandConfig.NumberOfSamplesInEnergySet = bd_DEF_NUMBER_OF_SAMPLES_IN_ENERGY_SET;
    configuration->FuFreqBandConfig.NumberOfEnergySets = bd_DEF_NUMBER_OF_ENERGY_SETS;
    configuration->FuFreqBandConfig.BeatLevelFactor = bd_DEF_BEAT_LEVEL_FACTOR;
    configuration->FuFreqBandConfig.VarianceFactor = bd_DEF_VARIANCE_FACTOR;
    
    configuration->LoFreqBandConfig = configuration->FuFreqBandConfig;
    configuration->HiFreqBandConfig = configuration->FuFreqBandConfig;
    
    SetBeatDetectorCutoffFrequencies(configuration, bd_DEF_LO_PASS_CUTOFF_FREQ, bd_DEF_HI_PASS_CUTOFF_FREQ);
}

void SetBeatDetectorCutoffFrequencies(BeatDetectorConfiguration_t *configuration, double loCutoffFrequency, double hiCutoffFrequency)
{
    double loPassRC = 1.0 / (2.0 * M_PI * loCutoffFrequency);
    double hiPassRC = 1.0 / (2.0 * M_PI * hiCutoffFrequency);
    
    configuration->LoPassFilterAlpha = ADC_DT / (loPassRC + ADC_DT);
    configuration->LoPassFilterOneMinusAlpha = 1.0 - configuration->LoPassFilterAlpha;
    configuration->HiPassFilterAlpha = hiPassRC / (hiPassRC + ADC_DT);
}

void InitBeatDetector(BeatDetector_t *beatDetector, BeatDetectorConfiguration_t *configuration)
{
    SetBeatDetectorConfiguration(beatDetector, configuration);
}

void SetBeatDetectorConfiguration(BeatDetector_t *beatDetector, BeatDetectorConfiguration_t *configuration)
{
    beatDetector->Configuration = configuration;
    ResetBeatDetector(beatDetector);
}

void ResetBeatDetector(BeatDetector_t *beatDetector)
{
    InitSoundEnergyData(&beatDetector->FuFreqSoundEnergyData, beatDetector->_fuFreqSoundEnergyHistory);
    InitSoundEnergyData(&beatDetector->LoFreqSoundEnergyData, beatDetector->_loFreqSoundEnergyHistory);
    InitSoundEnergyData(&beatDetector->HiFreqSoundEnergyData, beatDetector->_hiFreqSoundEnergyHistory);
    
    beatDetector->PreviousSample = 0;
    beatDetector->CurrentSample = 0;
}

void ProcessSample(BeatDetector_t *beatDetector, double sampleValue)
{
    StoreSample(beatDetector, sampleValue);
    FilterSample(beatDetector);
    AgregateSample(beatDetector);
}

void StoreSample(BeatDetector_t *beatDetector, double sampleValue)
{
    beatDetector->PreviousSample = beatDetector->CurrentSample;
    beatDetector->CurrentSample = sampleValue;
}

void FilterSample(BeatDetector_t *beatDetector)
{
    //LoPassSample(beatDetector);
    //HiPassSample(beatDetector);
}

void LoPassSample(BeatDetector_t *beatDetector)
{
    double currValue = beatDetector->CurrentSample;
    double currFilteredValues[bd_lopass_FILTER_ORDER];
    
    for (uint8_t orderIx = 0; orderIx < bd_lopass_FILTER_ORDER; ++orderIx)
    {
        if (currValue < .000001)
            currValue = 0;
        
        double filteredValue = beatDetector->Configuration->LoPassFilterAlpha * currValue;// + beatDetector->Configuration->LoPassFilterOneMinusAlpha * beatDetector->PreviousLoPassedValues[orderIx];
        if (isnanf(filteredValue))
            filteredValue = 0;
        currFilteredValues[orderIx] = filteredValue;
        currValue = currFilteredValues[orderIx];
    }
    
    for (uint8_t orderIx = 0; orderIx < bd_lopass_FILTER_ORDER; ++orderIx)
    {
        beatDetector->PreviousLoPassedValues[orderIx] = currFilteredValues[orderIx];
    }
}

void HiPassSample(BeatDetector_t *beatDetector)
{
    double currValue = beatDetector->CurrentSample;
    double prevValue = beatDetector->PreviousSample;
    double currFilteredValues[bd_hipass_FILTER_ORDER];
    
    for (int orderIx = 0; orderIx < bd_hipass_FILTER_ORDER; ++orderIx)
    {
        currFilteredValues[orderIx] = beatDetector->Configuration->HiPassFilterAlpha * beatDetector->PreviousHiPassedValues[orderIx] + beatDetector->Configuration->HiPassFilterAlpha * (currValue - prevValue);
        prevValue = beatDetector->PreviousHiPassedValues[orderIx];
        currValue = currFilteredValues[orderIx];
    }
    
    for (int orderIx = 0; orderIx < bd_hipass_FILTER_ORDER; ++orderIx)
    {
        beatDetector->PreviousHiPassedValues[orderIx] = currFilteredValues[orderIx];
    }
}

void AgregateSample(BeatDetector_t *beatDetector)
{
    AgregateSampleForSingleBand(&beatDetector->FuFreqSoundEnergyData, beatDetector->Configuration->FuFreqBandConfig.NumberOfSamplesInEnergySet, beatDetector->CurrentSample);
    AgregateSampleForSingleBand(&beatDetector->LoFreqSoundEnergyData, beatDetector->Configuration->LoFreqBandConfig.NumberOfSamplesInEnergySet, beatDetector->PreviousLoPassedValues[bd_lopass_FILTER_ORDER - 1]);
    AgregateSampleForSingleBand(&beatDetector->HiFreqSoundEnergyData, beatDetector->Configuration->HiFreqBandConfig.NumberOfSamplesInEnergySet, beatDetector->PreviousHiPassedValues[bd_hipass_FILTER_ORDER - 1]);
}

void AgregateSampleForSingleBand(SoundEnergyData_t *singleBandSoundEnergyData, uint8_t numberOfSamplesInEnergySet, double normalizedSampleValue)
{
    if (!singleBandSoundEnergyData->NewSetAwait)
    {
        singleBandSoundEnergyData->CurrentValue += pow(normalizedSampleValue, 2.0);
        
        if (++singleBandSoundEnergyData->SampleIndex == numberOfSamplesInEnergySet)
        {
            singleBandSoundEnergyData->NewSetAwait = TRUE;
        }
    }
}

void GetBeatDetectorState(BeatDetector_t *beatDetector, SingleBandBeatDetectorConfiguration_t *bandConfig, SoundEnergyData_t *soundEnergyData, BeatDetectorState_t *outputState)
{
    if (soundEnergyData->NewSetAwait)
    {
        double avgEnergy = 0;
        double *history = soundEnergyData->History;
        uint8_t numberOfEnergySets = bandConfig->NumberOfEnergySets;
        
        for (int i = 0; i < numberOfEnergySets; ++i)
            avgEnergy += history[i];
        avgEnergy /= numberOfEnergySets;
        
        double v = pow(history[0] - avgEnergy, 2);
        for (int i = 1; i < numberOfEnergySets; ++i)
        {
            v += pow(history[i] - avgEnergy, 2);
            history[i] = history[i - 1];
        }
        history[0] = soundEnergyData->CurrentValue;
        
        v /= numberOfEnergySets;
        double c = bandConfig->BeatLevelFactor - v * bandConfig->VarianceFactor;
        
        if (soundEnergyData->CurrentValue > c * avgEnergy)
        {
            outputState->IsDetected = TRUE;
            outputState->BeatPower = 1.0;
        }
        else
        {
            outputState->IsDetected = FALSE;
        }
        
        outputState->IsReady = TRUE;
        outputState->AverageEnergy = avgEnergy;
        outputState->CurrentEnergy = soundEnergyData->CurrentValue;
        
        ResetSoundEnergyData(soundEnergyData);
    }
    else
    {
        outputState->IsReady = FALSE;
    }
}