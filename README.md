<p align="center">
  <a href="https://github.com/blapaz/buddy">
    <img src="https://avatars3.githubusercontent.com/u/21316645?s=460&v=4" alt="Logo" width="80" height="80">
  </a>

  <h2 align="center">Buddy</h2>
  <p align="center">
    Buddy is an app that runs custom scripts for quick and easy interaction with Window's events
    <br />
    <a href="https://github.com/blapaz/buddy/issues/new">Request Feature</a>
    ·
    <a href="https://github.com/blapaz/buddy/wiki">Wiki</a>
    ·
    <a href="https://github.com/blapaz/buddy/issues/new">Report Bug</a>
  </p>
</p>

## About

Buddy is an experimental project designed to make interaction with Windows events easier. Scripts can be written in a simple language to interact with these Windows events. 

At its core, Buddy's main purpose is listening for hotkeys that can run a task (eg. taking a screenshot when a specific hotkey combination is pressed). However, the scripting can be used to extended further beyond that. Refer to the [wiki](https://github.com/blapaz/buddy/wiki) for more information.

One goal for this project over time, would be to have a well developed and stable language available with many system functions built in.

This project contains a **compiler**, **runtime** and a **bundle** (compiles code then runs it). There are libraries for the compiler and the runtime. The project was structured this way to allow for individual parts to be used or all together, depending on the scenario.

## Example

```
import system

global clips = [ "", "", "", "", "" ]

function Main()
{
  Message("Buddy script to store the history of the clipboard.")
}

event Control_C() 
{ 
  i = 4
	
  while (i > 0)
  {
    x = i - 1
    clips[i] = clips[x]
    i = x
  }
	
  clips[0] = GetClipboard()
}

event Control_Shift_F1() { Write(clips[0]) }
event Control_Shift_F2() { Write(clips[1]) }
event Control_Shift_F3() { Write(clips[2]) }
event Control_Shift_F4() { Write(clips[3]) }
event Control_Shift_F5() { Write(clips[4]) }
```

When this script is executed:
- The import will bring in system functions (ie. ```GetClipboard()```, ```Write()```, ```Message()```)
- The main function ```Main()``` will run immediately and display a message box
- The event function ```Control_C()``` will run when the key combination of Control and C is pressed
  - This will shift all the items in the clipboard history array over so that index 0 is always the most recent item copied.
- The events ```Control_Shift_<function-key>``` will write out the text saved in the corresponding point in the history.
  - The text will be pasted out just like the standard windows Control+V combination.
  
_For more examples and useful scripts, please refer to the [buddy-scripts repo](https://github.com/blapaz/buddy-scripts)_

## Reference
The compiler and runtime for this project were based on [Klip](https://github.com/TimeLoad00/Klip). 

Current open source libraries in this project:
1. [Global Mouse Key Hook](https://github.com/gmamaladze/globalmousekeyhook)
2. [Input Simulator](https://github.com/michaelnoonan/inputsimulator)

## Roadmap

See the [open issues](https://github.com/blapaz/buddy/issues) for a list of proposed features (and known issues).

## Contributing

Right now things are pretty limited, but additions are being made to improve the scripting and applications. This project could be used for practical purposes but is mainly a way to experiment and learn. Feel free to contribute and help grow the project if you are interested.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/<feature>`)
3. Commit your Changes (`git commit -m 'Add <feature>'`)
4. Push to the Branch (`git push origin feature/<feature>`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.