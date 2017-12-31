/*
 * SpectralAnalysis.c
 *
 * Created: 2013-03-10 14:00:47
 *  Author: pbejger
 */ 

#include "SpectralAnalysis.h"

void InitSAD(SpectralAnalysisData_t *spectralAnalysisData)
{
    spectralAnalysisData->DataCollector = spectralAnalysisData->DataBuffers[0];
    spectralAnalysisData->Data = spectralAnalysisData->DataBuffers[1];
    spectralAnalysisData->MaxSpectMag = 0;
    spectralAnalysisData->MaxOctaveSpectMag = 0;
    spectralAnalysisData->FirstOctaveLFreq = __sa_SAMPLE_FREQ_SPAN * pow(2.0, -.5);
}

void SwitchBuffer(SpectralAnalysisData_t *spectralAnalysisData)
{
    double *tmp = spectralAnalysisData->Data;
    spectralAnalysisData->Data = spectralAnalysisData->DataCollector;
    spectralAnalysisData->DataCollector = tmp;
}

void MakeAnalysis(SpectralAnalysisData_t *spectralAnalysisData)
{
    PrepareSignal(spectralAnalysisData->Data);

    Four1(spectralAnalysisData->Data, __sa_SAMPLE_COUNT, 1);

    TransformToMagnitudes(spectralAnalysisData->Data, &spectralAnalysisData->MaxSpectMag);

    DropOctaves(spectralAnalysisData->Octaves);

    DenormalizeOctaves(spectralAnalysisData->Octaves, spectralAnalysisData->MaxOctaveSpectMag);

    CalculateOctaves(spectralAnalysisData->Data, spectralAnalysisData->Octaves, &spectralAnalysisData->MaxOctaveSpectMag, spectralAnalysisData->FirstOctaveLFreq);

    NormalizeSpectrumMags(spectralAnalysisData->Data, spectralAnalysisData->MaxSpectMag);
    NormalizeOctavesMags(spectralAnalysisData->Octaves, spectralAnalysisData->MaxOctaveSpectMag);

    DampMaxSpectrumMags(&spectralAnalysisData->MaxSpectMag, &spectralAnalysisData->MaxOctaveSpectMag);
}

void PrepareSignal(double *signal)
{
    // REMOVE DC FROM SIGNAL
    // By doing this operation we gets useful information from first bin of spectrum (f = 0Hz)
    // example:
    // signal frequency = 5Hz
    // first bin = 0Hz, second bin = 10Hz
    // in this case half of power of signal will be represented by 0Hz bin, and half of power by 10Hz bin

    for (int dIx = 0; dIx < __sa_SPECTRUM_SIZE; dIx += 2)
    {
        signal[dIx] -= __sa_HALF_OF_MAX_SIGNAL_VALUE;
    }
}

void TransformToMagnitudes(double *spectrum, double *maxMagnitude)
{
    // From performance reasons this operation is performed only for first half of spectrum bins.
    
    for (int dIx = 0; dIx < __sa_SPECTRUM_SIZE_DIV2; dIx += 2)
    {
        double re = spectrum[dIx] / __sa_MAX_SIGNAL_VALUE;
        double im = spectrum[dIx + 1] / __sa_MAX_SIGNAL_VALUE;
        double mag = sqrt(pow(re, 2.0) + pow(im, 2.0));
        spectrum[dIx] = mag;

        if (mag > *maxMagnitude)
            *maxMagnitude = mag;
    }
}

void DropOctaves(double *normalizedOctaves)
{
    for (int i = 0; i < __sa_OCTAVE_COUNT; ++i)
        normalizedOctaves[i] -= __sa_OCTAVE_DROP;
}

void DenormalizeOctaves(double *normalizedOctaves, double maxOctaveSpectMag)
{
    for (int i = 0; i < __sa_OCTAVE_COUNT; ++i)
        normalizedOctaves[i] *= maxOctaveSpectMag;
}

void CalculateOctaves(double *magnitudeSpectrum, double *octaves, double *maxMagnitude, double firstOctaveLFreq)
{
    int octaveIndex = 0;
    double octaveMagSum = 0;

    double octaveLFreq = 0;
    double octaveRFreq = firstOctaveLFreq;

    for (int i = 0; i < __sa_SAMPLE_COUNT_DIV2; ++i)
    {
        double mag = magnitudeSpectrum[i * 2];

        double freq = i * __sa_SAMPLE_FREQ_SPAN;

        if (freq > octaveRFreq || i == __sa_SAMPLE_COUNT_DIV2 - 1)
        {
            if (i == __sa_SAMPLE_COUNT_DIV2 - 1)
                octaveRFreq = __sa_HALF_OF_SAMPLING_FREQUENCY;

            if (octaveMagSum > octaves[octaveIndex])
                octaves[octaveIndex] = octaveMagSum;
            if (octaveMagSum > *maxMagnitude)
                *maxMagnitude = octaveMagSum;
            octaveMagSum = 0;

            octaveLFreq = octaveRFreq;
            octaveRFreq *= 2;

            ++octaveIndex;
        }

        octaveMagSum += mag * mag;
    }
}

void NormalizeSpectrumMags(double *magnitudeSpectrum, double maxSpectMag)
{
    // From performance reasons this operation is performed only for first half of spectrum bins.
    
    for (int i = 0; i < __sa_SPECTRUM_SIZE_DIV2; i += 2)
        magnitudeSpectrum[i] /= maxSpectMag;
}

void NormalizeOctavesMags(double *octaves, double maxOctaveSpectMag)
{
    for (int i = 0; i < __sa_OCTAVE_COUNT; ++i)
        octaves[i] /= maxOctaveSpectMag;
}

void DampMaxSpectrumMags(double *maxSpectMag, double *maxOctaveSpectMag)
{
    *maxSpectMag *= __sa_MAX_SPECT_MAG_DAMPING;
    *maxOctaveSpectMag *= __sa_MAX_OCTAVE_SPECT_MAG_DAMPING;
}
