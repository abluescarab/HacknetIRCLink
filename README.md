# HacknetIRCLink

This is a [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder) mod for Hacknet that lets you communicate via IRC.
Messages are displayed in the command line.

In order to use the mod you must **put ChatSharp.dll in the same folder as Hacknet.exe**.

## Usage
* `irc link <server> <#channel>`: link to a server (Ex: eu.iso.rizon.net #hacknetlink)
* `irc connect [server] [#channel]`: without arguments, connect to the linked server; with arguments, connect to the specified server
* `irc disconnect`: disconnect from a server
* `irc switch <#channel>`: switch channels
* `irc raw <message>`: send a [raw message](https://en.wikipedia.org/wiki/List_of_Internet_Relay_Chat_commands) to the server
* `irc help`: display in-game help
* `say <message>`: send a message to the IRC channel
