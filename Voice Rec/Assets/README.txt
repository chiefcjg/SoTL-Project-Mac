Notes and Tips:

In the git hub there is links setup to make access to azure quicker and more functional.

There is the original audio clips used in testing. 
While no longer needed they are included in case of in the future clips are wanted.

There is also a unity package file that is the microsoft Azure SDK. 
When it is unpacked in unity it brings in both IOS and Android which brings in files that are too big to import into github.
Make sure to delete for easier upload and download.

In the Scripts folder is two scripts

SpeechToText which is grabbing speech from the default mic and using it.
	First thing about this script is you need to remember to use a different key and server location when resetting it back up under a new account.
	This is easy as it is simply make an azure account, makea speech service for yourself and copy paste. if confused go to links and follow the steps of speech to text.

	Secound part of this script is at the bottom it compares what you set as an answer to what is recorded. important to note here is that this follows Azure's grammar ways.
	Meaning that the first letter needs to be capital and ends with the correct punctuation.
	(this may be changed due to important new features with Stephen)

	Final note: This is not as scary as it actually seems. most of it is drag and drop when bringing it into a new scene now. 
	main thing is to ensure there is the button to press.

TextToSpeech is grabbing a text block and reading it back
	Same as SpeechToText, First thing about this script is you need to remember to use a different key and server location when resetting it back up under a new account.
	This is easy as it is simply make an azure account, makea speech service for yourself and copy paste. if confused go to links and follow the steps of speech to text.

	Secoundly this is all azures code mainly of how they have their default readings. 
	I added comments where i could to help but most of this is basic what azure needs to run.

There is also a printed PDF website of the degrees of similarity that Stephen came across.
This is linked as to help provide some more insight as to how this project works.

Overall this readme file and the comments in the code should help you navigate what does what very simply.
This can all be changed as well depending on when this has last been updated.
I will not be including any of my notes, mockups, and ideas for how to do this sort of coding as better resources and ways to do so have been found since i started.

Created:
06/10/2020
MacKenzie Ackles

There is a new file/script called SpeechToTextNewest.

This script is the combined efforts of Stephen Graham and MacKenzie Ackles.

comments have been updated in it to provide best knowledge.

This script requires SteamVR to be installed as well as usable. 
The project has needs some changes inside of unity as well. 
The provided Link can walk you through the steps.
https://medium.com/@sarthakghosh/a-complete-guide-to-the-steamvr-2-0-input-system-in-unity-380e3b1b3311
will also download as a pdf and include in GIT.

The action made is called RecordMic, and the new action set was acalled p3v.
After being set dont forget to also add the script to the VR Player, as well as
adding Steam VR_Activate Action Set On Load.

VRPlayer needs to be deleted after you start loading a scene inorder to load.

For any more info and assistance consult the link and saved PDF

Last Updated:
06/18/2020
MacKenzie Ackles
