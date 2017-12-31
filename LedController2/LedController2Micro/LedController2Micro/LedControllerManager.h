/*
 * LedControllerManager.h
 *
 * Created: 2013-08-17 22:15:35
 *  Author: PiotrB
 */ 


#ifndef LEDCONTROLLERMANAGER_H_
#define LEDCONTROLLERMANAGER_H_

#include <stdint.h>
#include <avr/cpufunc.h>
#include <stddef.h>

// TYPE PROTOTYPES

typedef enum StatusFlagIndex
{
	lcm_sfix_IS_SYSTEM_ENABLED = 0,
	lcm_sfix_IS_SOUND_ENABLED = 1,
} __attribute__((packed)) StatusFlagIndex_t;

struct LedControllerManager;

typedef struct LedControllerManager LedControllerManager_t;

struct LedControllerManager
{
	void (*TurnOn)();
	
	void (*TurnOff)();
	
	void (*SoundOn)();
	
	void (*SoundOff)();
	
	uint8_t IsSystemEnabled;
	
	uint8_t IsSoundEnabled;
};

// FUNCTION DECLARATIONS

void InitLedControllerManager(LedControllerManager_t *lcManager);

#endif /* LEDCONTROLLERMANAGER_H_ */