# HacknetIRCLink
In order to use the mod you HAVE to put chatsharp.dll in the same folder as Hacknet.exe.

This is a [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder) mod for Hacknet that lets you communicate via IRC. 
Messages are displayed in the command line.

To change the irc settings and to start/stop the client use the irc command.
The arguments are as follows:
* help: gives you instructions on how to use the mod (*irc help*)
* link: binds the mod to a server and a channel (*irc link [SERVER] [CHANNEL]* or *irc link* to display the linked server)
* connect: connects to a server of your choosing (*irc connect [SERVER] [CHANNEL]* or *irc connect* to connect to the linked server)
* disconnect: pretty self explenatory (*irc disconnect*)
* switch: changes your channel (*irc switch [CHANNEL]*)
* raw: sends a raw message to the channel you're connected to (*irc raw [RAW MESSAGE]*). See [here](https://en.wikipedia.org/wiki/List_of_Internet_Relay_Chat_commands) for a list of commands.
To send messages use the "say" command.

