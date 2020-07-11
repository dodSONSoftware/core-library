**dodSON Software Core Library**

The dodSON Software Core Library abstracts common programming tasks into sets of well researched, developed and documented types organized into separate, self-contained namespaces enabling rapid development of feature-rich applications and services which can be used independently. However, used together these namespaces provide the foundation for a configurable, packaged-deployed, plugin-based distributed programming framework with remote monitoring and control, standardized configuration, packaging, installation, logging and networking.

### Help File

Download the [dodSON Software Core Library Help](https://1drv.ms/u/s!Ap-545RFn6fujTYHfNDwAxYQ9Yc8?e=iTWh1e) file for technical details and examples.

### Prerequisites

This library was last built against **.Net Framework 4.6**.

To compile for **.Net Core** you must remove the following namespaces:
  * Addon
  * AppDomain
  * ComponentManagement
  * ServiceManagement
  * ServiceManagement.RequestResponseTypes

This is because **.Net Core** does not have the concept of application domains; all of the distributed framework aspects of the dodSON Software Core Library is built on application domains which originates in the AppDomain namespace.

## Built With

* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) - Visual Studio 2019 [Community Edition]
* [.Net Frameworks](https://dotnet.microsoft.com/download/dotnet-framework) - .Net Frameworks

## Authors

* **Randy Dodson (dodSON Software)** - Please visit my [YouTube Channel](https://www.youtube.com/channel/UCj_glv7KHILjPaQxSw8a3Cg) for videos detailing how to use this library.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Please, feel free to create feature branches and make them your own. Nothing would please me more than to see this library grow.
