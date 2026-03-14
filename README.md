# Windows 10 CD installer

<img width="979" height="510" alt="image" src="https://github.com/user-attachments/assets/f0f60e82-60d8-406d-9097-2cd7907a0ad6" />

This is a random project I made in C#.
To use this, you'll need to split a Windows 10 1809 x64 ISO into 700MB .swm files.

Use the following command to do this:
dism /Split-Image /ImageFile:install.wim /SWMFile:install.swm /FileSize:700 (change ImageFile and SWMFile path if needed)

You'll need to place the individual .swm files into an individual ISO file using a program like AnyBurn (https://anyburn.com/).
Make sure to rename the 1st install.swm to install1.swm (sorry for this, but..)

Run the setup program from Windows PE or a regular install of Windows and follow the instructions.
