# Aurora Box

![logo](./logo.png)

aurora-box is an experimental CLI tool that generates static data from the League Wiki's ARAM and Arena page. Generated data currently includes champion balance changes and effects for each mode.

Made using C# and the .NET ecosystem.

## Contributors

<a href="https://github.com/BlossomiShymae/AuroraBox/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=BlossomiShymae/AuroraBox" />
</a>

## Requirements

- .NET 8 capable runtime
- x64 Windows, Linux, or Mac

## Usage

```bash
./aurora-box
```

When it successfully runs, data files will be written to the local folder. 

MessagePack can be selected as the serialized data format instead of JSON:

```bash
./aurora-box -f MessagePack
```

### Linux

If you're running a Ubuntu distro, add the runtime if not installed:

```sudo apt-get install dotnet-runtime-8.0```

Set executable permissions:

```chmod +x ./aurora-box```

## License

AuroraBox is licensed under the terms of the MIT license.