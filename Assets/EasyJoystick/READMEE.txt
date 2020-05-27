Easy joystick v 1.0

To create a ready for use joystick go to GameObject -> UI -> Joystick.

How to use:

1) Declare public EasyJoystick variable and assign created joystick to it.
2) There are 4 public functions to use:
					EasyJoystick.MoveInput() - returns vector3 for movement;

					EasyJoystick.Rotate(transformToRotate, rotateSpeed) - Rotation function, usefull for look rotation;
					
					EasyJoystick.IsPressed() - returns true if joystick is pressed and conversely, usefull for fire functions and so on.
					
					EasyJoystick.Enable(boolean) - enable or disable joystick, use this to disable and hide controlls;

See example Movement script in the Demo assets for more understanding. Also dont forget to check a setup 
video if you didn't yet.