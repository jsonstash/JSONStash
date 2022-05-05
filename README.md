# jsonstash

A free self-hosted JSON storage solution.

## Getting Started

Learn about the endpoints at [docs.jsonstash.com](https://docs.jsonstash.com) or check out the postman workspace:

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/4246898-d0c0ef60-166a-4b96-8400-03783f22129f?action=collection%2Ffork&collection-url=entityId%3D4246898-d0c0ef60-166a-4b96-8400-03783f22129f%26entityType%3Dcollection%26workspaceId%3D313a790d-ec68-4110-b509-4902e6397a1a)


### Prerequisities


In order to run this container you'll need docker installed.

* [Windows](https://docs.docker.com/windows/started)
* [OS X](https://docs.docker.com/mac/started/)
* [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Environment Variables

* `DefaultConnection` - The path where you want the sqlite database stored.
* `JSONMaxBytes` - Max size of json allowed to be stashed.
* `StashNameMaxLength` - Max number of chars allowed to be used when naming a stash.
* `LoginAttemptThreshold` - Max number of failed login attempts before locking the user out.
* `JWTSecret` - Json Web Token secret.
* `JWTExpiresIn` - Json Web Token amount of time in minutes the token expires.

#### Volumes

* `/your/file/location` - File location

## Built With

* C# .NET 6
* [Newtonsoft.Json v13.0.1](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1)
* [SQLite](https://www.sqlite.org/index.html)

## Find Us

* [Docker](https://hub.docker.com/r/kerberusio/jsonstash)

## Authors

* **Lincoln Hach Yellick**  - [LinkedIn](https://www.linkedin.com/in/lyellick/)

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE.md](https://github.com/jsonstash/JSONStash/blob/main/LICENSE) file for details.

## Acknowledgments

* Thanks to my wife and kids who I love. They didn't help with this at all, but I just like saying it.
