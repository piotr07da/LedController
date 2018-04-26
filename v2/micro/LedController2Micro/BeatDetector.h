/*
 * BeatDetector.h
 *
 * Created: 2013-05-25 15:51:31
 *  Author: pbejger
 */ 

#ifndef BEATDETECTOR_H_
#define BEATDETECTOR_H_

#include "GlobalConstants.h"

#include <stdint.h>
#include <stddef.h>
#include <math.h>

#define bd_fuf_MAX_NUMBER_OF_ENERGY_SETS 96
#define bd_lof_MAX_NUMBER_OF_ENERGY_SETS 96
#define bd_hif_MAX_NUMBER_OF_ENERGY_SETS 96

#define bd_lopass_FILTER_ORDER 3
#define bd_hipass_FILTER_ORDER 2

#define bd_DEF_NUMBER_OF_SAMPLES_IN_ENERGY_SET 150
#define bd_DEF_NUMBER_OF_ENERGY_SETS 58
#define bd_DEF_BEAT_LEVEL_FACTOR 2.0142857
#define bd_DEF_VARIANCE_FACTOR 0.0015714
#define bd_DEF_LO_PASS_CUTOFF_FREQ 600
#define bd_DEF_HI_PASS_CUTOFF_FREQ 3800

//typedef enum FramePart
//{
    //com_fp_SYNCWORD = -1,
    //com_fp_TYPE_LENGTH = 0,
    //com_fp_PAYLOAD = 1,
    //com_fp_FRAME_CHECK_SEQUENCE = 2,
//} __attribute__((packed)) FramePart_t;

struct SoundEnergyData;
struct BeatDetector;
struct SingleBandBeatDetectorConfiguration;
struct BeatDetectorConfiguration;
struct BeatDetectorState;

typedef struct SoundEnergyData SoundEnergyData_t;
typedef struct BeatDetector BeatDetector_t;
typedef struct SingleBandBeatDetectorConfiguration SingleBandBeatDetectorConfiguration_t;
typedef struct BeatDetectorConfiguration BeatDetectorConfiguration_t;
typedef struct BeatDetectorState BeatDetectorState_t;

struct SoundEnergyData
{
    double *History;
    
    double CurrentValue;
    
    uint8_t SampleIndex;
    
    uint8_t NewSetAwait;
};

struct BeatDetector
{
    double _fuFreqSoundEnergyHistory[bd_fuf_MAX_NUMBER_OF_ENERGY_SETS];
    
    double _loFreqSoundEnergyHistory[bd_lof_MAX_NUMBER_OF_ENERGY_SETS];
    
    double _hiFreqSoundEnergyHistory[bd_hif_MAX_NUMBER_OF_ENERGY_SETS];
    
    // Configuration.
    BeatDetectorConfiguration_t *Configuration;
    
    SoundEnergyData_t FuFreqSoundEnergyData;
    
    SoundEnergyData_t LoFreqSoundEnergyData;
    
    SoundEnergyData_t HiFreqSoundEnergyData;
    
    double PreviousLoPassedValues[bd_lopass_FILTER_ORDER];
    
    double PreviousHiPassedValues[bd_hipass_FILTER_ORDER];
    
    // Original value of previous sample.
    double PreviousSample;
    
    // Original value of current sample.
    double CurrentSample;
};

struct SingleBandBeatDetectorConfiguration
{
    uint8_t NumberOfSamplesInEnergySet;
    
    uint8_t NumberOfEnergySets;
    
    double BeatLevelFactor;
    
    double VarianceFactor;
};

// Configuration:
// - number of samples in sound energy set (per: full, low, high frequencies)
// - number of sound energy sets (per: full, low, high frequencies)
// - beat-level factor (per: full, low, high frequencies)
// - variance factor (per: full, low, high frequencies)
// - lo-pass cutoff frequency
// - hi-pass cutoff frequency
struct BeatDetectorConfiguration
{
    // Full frequency band beat detector configuration
    SingleBandBeatDetectorConfiguration_t FuFreqBandConfig;
    
    // Low frequency band beat detector configuration
    SingleBandBeatDetectorConfiguration_t LoFreqBandConfig;
    
    // High frequency band beat detector configuration
    SingleBandBeatDetectorConfiguration_t HiFreqBandConfig;
    
    // Low pass filter alpha.
    // Alpha = dt / (rc + dt)
    // where rc is rc calculated for cutoff frequency for low/band-pass filter.
    double LoPassFilterAlpha;
    
    // Low pass filter 1 - alpha.
    double LoPassFilterOneMinusAlpha;
    
    // High pass filter alpha.
    // Alpha = rc / (rc + dt)
    // where rc is rc calculated for cutoff frequency for high/band-pass filter.
    double HiPassFilterAlpha;
};

struct BeatDetectorState
{
    // Value indicating whether the information about beat is ready to read.
    uint8_t IsReady;
    
    // Value indicating whether the beat has been detected.
    uint8_t IsDetected;
    
    // Power of beat.
    double BeatPower;
    
    // Current energy of sound (value of current energy set).
    double CurrentEnergy;
    
    // Average energy of sound (calculated from all stored energy sets).
    double AverageEnergy;
    
    // Current variance of sound energy.
    double Variance;
};

struct XXX { uint8_t a; };

// SOUND ENERGY DATA METHODS

void InitSoundEnergyData(SoundEnergyData_t *data, double *historyBuffer);

void ResetSoundEnergyData(SoundEnergyData_t *data);

// BEAT DETECTOR CONFIGURATION METHODS

void InitBeatDetectorConfiguration(BeatDetectorConfiguration_t *configuration);

void SetBeatDetectorCutoffFrequencies(BeatDetectorConfiguration_t *configuration, double loCutoffFrequency, double hiCutoffFrequency);

// BEAT DETECTOR METHODS

void InitBeatDetector(BeatDetector_t *beatDetector, BeatDetectorConfiguration_t *configuration);

void SetBeatDetectorConfiguration(BeatDetector_t *beatDetector, BeatDetectorConfiguration_t *configuration);

void ResetBeatDetector(BeatDetector_t *beatDetector);

void ProcessSample(BeatDetector_t *beatDetector, double sampleValue);

void StoreSample(BeatDetector_t *beatDetector, double sampleValue);

void FilterSample(BeatDetector_t *beatDetector);

void LoPassSample(BeatDetector_t *beatDetector);

void HiPassSample(BeatDetector_t *beatDetector);

void AgregateSample(BeatDetector_t *beatDetector);

void AgregateSampleForSingleBand(SoundEnergyData_t *singleBandSoundEnergyData, uint8_t numberOfSamplesInEnergySet, double normalizedSampleValue);

void GetBeatDetectorState(BeatDetector_t *beatDetector, SingleBandBeatDetectorConfiguration_t *bandConfig, SoundEnergyData_t *soundEnergyData, BeatDetectorState_t *outputState);

#endif /* BEATDETECTOR_H_ */