# ARModelView
An AR prototype to interact with 3D objects.

# App Use
The app was developed for Android mobile devices. To immediately  use the app, there is a `.apk` build in `ARModels/AndroidBuild` that can be installed on an Android smartphone. 

The app initializes by showing the instructions panel to the user. 
Once the instructions panel is closed, the user can tap on AR planes to add objects. There is also a Menu bar to:
- Toggle ARNotes view
- Show AR planes
- Select different objects to add
- Toggle physics interaction 
- Delete all objects
- Reopen the instructions panel

![Screenshot3](https://github.com/user-attachments/assets/ac1c51de-6d79-4aa2-b9df-d034abdc2449)





When an object is selected (by tapping on it), the user can:
- Rotate, scale and move the object with the fingers interaction.
- Also, it can press the "Click to speak button" to record simple commands such as "forward", "rotate" and "scale".


Due to the package used and device limitations, voice command recognition may take several seconds. 
To use voice commands, the user must:

1. Tap the button
2. Say the command ("forward", "rotate" or "scale")
3. Tap again in the same button.

The voice recorded will then play and process speech-to-command to the object.

![Screenshot2](https://github.com/user-attachments/assets/e58f410f-bac8-4c1e-9645-a2ec53f2f147)
![Screenshot5](https://github.com/user-attachments/assets/6a74c2f7-8920-4f27-9b12-e0d099984acf)


Each object has an ARNote displayed above it, which contains its name and the time it was added to the world.

The AR planes have a segmentation by colors: 
- Blue -> floor
- Orange -> the walls
- Purple -> other horizontal surfaces.

### Physics Interaction: 

When Physics is activated, users will notice interactions when moving objects against each other. Also, vertically placed objects will fall.

### Occlusion Support: 

If the device used supports occlusion, AR objects will behave accordingly, blending with the real-world environment.

# Key decisions

In order to have all the required features, it was decided to organize the app by allowing different features with a Menu Bar, allowing the user to easily access and interact with the different functionalities in a user-friendly manner.

To maintain modular and organized code, several managers were used: `App State Manager`, `Menu Manager`, `Interactions Manager`, `Plane Generated Manager`, `Voice Commands Manager`. These managers handle different parts of the app, ensuring clear separation of responsibilities. 

To enable the AR on Android devices, it was used the AR Foundation package alongside the [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html) package. This combination allowed the implementation of gestures, interaction with the Objects and handle AR Notes. One challenge encountered was related to toggling ARNotes, where it seemed that an object selected is destroyed but a reference within the XR Interaction Toolkit would persist. Instead of using `Destroy()`, it was used `.SetActive(false)` to achieve a similar visual result.

To enable physics interaction between objects, a `RigidBody` is added to each Object.

Development and testing were conducted using a Samsung Galaxy A14 5G. This device does not support Occlusion but the project was set up to utilize this feature if the smartphone supports it. Another limitation of this device is the inability to use plane segmentation with AR Foundation. So, for devices that do not support it, it was added logic to predict the ARPlanes type:

- If plane is vertical, it is assumed to be a wall.
- The lowest horizontal plane is assumed to be the floor.
- Any horizontal plane above floor level is assumed to be a surface.

The objects used in the app were downloaded from the following sources: 
- [Bosch drill](https://skfb.ly/onCoH)
- [Bosch toolbox](https://3dwarehouse.sketchup.com/model/u9fcbb6a2-fa73-418b-ae5a-8b41a6b5846e/BOSCH-L-boxx-136)

Implementing voice commands was a challenging task due to the limited availability of suitable libraries for Android in Unity. After some research and testing, the [Whisper.unity](https://github.com/Macoron/whisper.unity) library seemed to facilitate the feature in question. The approach involved recording speech, converting it to text and recognizing specific keywords to trigger commands for the selected object. However, it seems that in the Android device tested, the speech-to-text conversion takes a few seconds due to the Whisper's processing time. A tiny model was also used for faster but less accurate results. Despite this, this was the best possible approach for this Android device.  

The project was developed with `Unity 2022.3.15f1` version.

