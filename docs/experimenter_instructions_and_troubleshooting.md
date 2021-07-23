# Experimenter Instructions and Troubleshooting

## Notes on Bonsai Workflow

- Starting/Stopping Cameras
	- start: shift + F1 
	- stop: shift + F2
- Starting Postion Tracking
	- click on overhead camera gui and move hand around, above the arena
- Hotkey triggering of pellets
	- patch 1: shift + F3
	- patch 2: shift + F4

- Things to pay attention in GUI:
	- Things to try if a beambreak signal is not seen after a pellet is triggered:

	![Pellet trigger no beambreak](pellet_trigger_no_beambreak.png)
		
		- Things to try if the FED3 is not delivering pellets when triggered:
			 - Clear any debris in the FED3 aperture.
			 - Clear any debris in the hole that feeds from the FED3 hopper to the FED3 aperture (underneath the disk).
			 - Clear any debris lodged underneath the disk.
			 - Ensure the disk fits snugly in the hopper and can be manually rotated.
			 - Clear any debris in the chute from the FED3 aperture to the wheel.
		- Things to try if the FED3 is delivering pellets when triggered:
			- Clear any debris from the FED3 aperture that may be breaking the beambreak (even though this debris is not stopping pellet delivery).
			- Restart the workflow and/or Bonsai.
	
	- Things to try if there is noise and/or fluctuations in the wheel's magnetic encoder values:

	![Magnetic encoder noise](Magnetic_encoder_noise.png)

		- Underneath the wheel, check that the magnet and the encoder PCB are in alignment.
		- Check that all wiring between the FED3, encoder PCB, and HARP device is properly connected.

- Notes on Errors:
	- Out-of-memory error
	- Tracking error (tracking queue empty)

## Starting a Session (Placing a mouse in the arena)

1. Manually trigger each patch multiple times to ensure pellets are being delivered properly.
2. Ensure parameter values in Bonsai workflow are properly set.
3. Take some bedding from the animal's cage and place it in the arena nesting box.
4. Place water in the arena.
5. Weigh the animal.
6. Place the animal in the arena nesting box.
7. In Bonsai, record the animal's ID and weight, and start the session.

## Ending a Session (Removing a mouse from the arena)
1. In Bonsai, stop the session.
2. Remove the animal from the arena and weight it.
3. In Bonsai, record the animal's weight and end the session.
4. Record the number of missed pellets (that fell through the wheel to the petry dish below the arena) for each patch for the session.
5. Clean the arena nesting box, corridor, and arena (particularly the food patch tiles) with 70% ethanol.

## General Maintenance

- Clean the chutes from the FED3 apertures to the wheels, weekly.
- Measure and record wheels' start torques, weekly.
- Replace 3d printed parts as necessary.

## Notes on Hardware Configuration

## Random Other Notes

- Fed3 beambreak is much more likely to be broken if something slows down the pellet on the way out from the Fed3 aperture.