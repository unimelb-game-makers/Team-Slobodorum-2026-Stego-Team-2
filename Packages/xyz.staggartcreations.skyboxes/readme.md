Copyright: Staggart Creations 2026

# Stylized Skybox HDRI Pack

Using skyboxes:
- Drag & drop any of the skybox materials into the scene-view to apply them.
OR
- Go to Window->Rendering->Lighting (Environment tab) and change the skybox material (all of them have the "Sky_" name prefix).

For HDRP, see the package's manual: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@10.3/manual/Override-HDRI-Sky.html

# Banding artefacts
You may notice banding artefacts on skyboxes. This is a normal aspect of 3D rendering.

It can be mitigated by enabling "Dithering" on the camera (URP/HDRP).

# Usage with mobile hardware
It could be benefitial to reduced the maximum resolution of the HDRI's for specific platforms.
See the Unity manual on how to achieve this: https://docs.unity3d.com/6000.3/Documentation/Manual/class-TextureImporter-type-specific.html

# License
Goverend by the the Unity Asset Store EULA. Skyboxes cannot be used in other Unity asset store packages, nor be used for AI content.