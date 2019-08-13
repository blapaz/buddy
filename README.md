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
    <a href="https://github.com/blapaz/buddy/issues/new">Report Bug</a>
  </p>
</p>

## About

Buddy is an experimental project designed to make interaction with Windows events easier. Scripts can be written in a simple language to interact with these Windows events. 

At its core, Buddy's main purpose is listening for hotkeys that can run a task (eg. taking a screenshot when a specific hotkey combination is pressed). However, the scripting can be used to extended further beyond that. 

One goal for this project over time, would be to have a well developed and stable language available with many system functions built in.

## Example

```
import system

function Main()
{
    PrintLine("Hello World")
}

event Control_C() 
{
	PrintLine("Clipboard")
}

event Control_W() 
{
	PrintLine("Control+W")
	Delay("5000")
	PrintLine("Screenshot")
	CaptureScreen("")
}
```

When this script is executed:
- The main function ```Main()``` will run immediately
- The event functions ```Control_C()``` and ```Control_W()``` will run when the corresponding key combinations are pressed 
  - Pressing Control and C will run ```Control_C()```
  - Pressing Control and W will run ```Control_W()```.

_For more examples, please refer to the [scripts folder](https://github.com/blapaz/buddy/tree/master/scripts) in the project_

## Roadmap

See the [open issues](https://github.com/blapaz/buddy/issues) for a list of proposed features (and known issues).

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/<feature>`)
3. Commit your Changes (`git commit -m 'Add <feature>'`)
4. Push to the Branch (`git push origin feature/<feature>`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.