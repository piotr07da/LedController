/*
 * SpectralAnalysis.h
 *
 * Created: 2013-02-14 22:53:53
 *  Author: pbejger
 */ 


#ifndef SPECTRALANALYSIS_H_
#define SPECTRALANALYSIS_H_

#include <math.h>

#include "Four1.h"
#include "GlobalConstants.h"

#define __sa_ANALOG_CLOCK_PRESCALER 128.0
#define __sa_SAMPLING_FREQ F_CPU / (__sa_ANALOG_CLOCK_PRESCALER * 13.0)
#define __sa_HALF_OF_SAMPLING_FREQUENCY __sa_SAMPLING_FREQ / 2.0
#define __sa_SAMPLE_TIME_SPAN 1.0 / __sa_SAMPLING_FREQ
#define __sa_SAMPLE_FREQ_SPAN __sa_SAMPLING_FREQ / __sa_SAMPLE_COUNT
#define __sa_SAMPLE_COUNT 64
#define __sa_SAMPLE_COUNT_DIV2 __sa_SAMPLE_COUNT / 2
#define __sa_SPECTRUM_SIZE __sa_SAMPLE_COUNT * 2
#define __sa_SPECTRUM_SIZE_DIV2 __sa_SPECTRUM_SIZE / 2
#define __sa_OCTAVE_COUNT 8
#define __sa_MAX_SPECT_MAG_DAMPING .953
#define __sa_MAX_OCTAVE_SPECT_MAG_DAMPING .953
#define __sa_OCTAVE_DROP 1.00 // multiplied by max octave amplitude
#define __sa_MAX_SIGNAL_VALUE 1023.0
#define __sa_HALF_OF_MAX_SIGNAL_VALUE __sa_MAX_SIGNAL_VALUE / 2.0


struct SpectralAnalysisData;

typedef struct SpectralAnalysisData SpectralAnalysisData_t;

struct SpectralAnalysisData
{
    uint8_t DataBufferIndex;
    
    double *Data;
    
    double *DataCollector;
    
    double DataBuffers[2][__sa_SPECTRUM_SIZE];
    
    double Octaves[__sa_OCTAVE_COUNT];
    
    double MaxSpectMag;
    
    double MaxOctaveSpectMag;
    
    double FirstOctaveLFreq;
};

void InitSAD(SpectralAnalysisData_t *spectralAnalysisData);
void SwitchBuffer(SpectralAnalysisData_t *spectralAnalysisData);
void MakeAnalysis(SpectralAnalysisData_t *spectralAnalysisData);
void PrepareSignal(double *signal);
void TransformToMagnitudes(double *spectrum, double *maxMagnitude);
void DropOctaves(double *normalizedOctaves);
void DenormalizeOctaves(double *normalizedOctaves, double maxOctaveSpectMag);
void CalculateOctaves(double *magnitudeSpectrum, double *octaves, double *maxMagnitude, double firstOctaveLFreq);
void NormalizeSpectrumMags(double *magnitudeSpectrum, double maxSpectMag);
void NormalizeOctavesMags(double *octaves, double maxOctaveSpectMag);
void DampMaxSpectrumMags(double *maxSpectMag, double *maxOctaveSpectMag);

#endif /* SPECTRALANALYSIS_H_ */