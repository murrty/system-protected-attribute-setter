# system-protected-attribute-setter
Sets the attribute "System Protected" to selected files/folders

this was mainly a test to look around the [ookii dialogs](https://github.com/caioproiete/ookii-dialogs-winforms) library, but i was reading on attribute flags and implemented it with the "system protected" attribute flag.

# how it works
the user gets to select to either hide files/folders or show files/folders, after the user makes their decision, they select the input they want to set as hidden. after they do so, it'll create a new runas process for itself with arguments for the action and the files. when it does, itll separate the files into a list, and then run through each of them.

the way it flags files is through cmd with runas, which gets redirected to the program so it can determine if it successfully set it or not. it's not perfect, but it works.

arguments are <-setattribute/-s> <true/false> <files/directories>, each item input is separate with a pipe "|".
