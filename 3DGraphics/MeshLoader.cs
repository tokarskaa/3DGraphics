using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.Mathematics.Interop;

namespace _3DGraphics
{
    static public class MeshLoader
    {
        public static async Task<Scene> LoadJSONFileAsync(string fileName)
        {
            var scene = new Scene();
            var meshes = new List<Mesh>();
            string text = String.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    text = await sr.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load mesh");
            }

            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(text);

            for (var meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                var verticesArray = jsonObject.meshes[meshIndex].positions;
                // Faces
                var indicesArray = jsonObject.meshes[meshIndex].indices;

                var normalsArray = jsonObject.meshes[meshIndex].normals;

                var verticesStep = 3;

                // the number of interesting vertices information for us
                var verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                var facesCount = indicesArray.Count / 3;
                var mesh = new Mesh(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount);

                // Filling the Vertices array of our mesh first
                for (var index = 0; index < verticesCount; index++)
                {
                    var x = (double)verticesArray[index * verticesStep].Value;
                    var y = (double)verticesArray[index * verticesStep + 1].Value;
                    var z = (double)verticesArray[index * verticesStep + 2].Value;
                    // Loading the vertex normal exported by Blender
                    var nx = (double)normalsArray[index * verticesStep].Value;
                    var ny = (double)normalsArray[index * verticesStep + 1].Value;
                    var nz = (double)normalsArray[index * verticesStep + 2].Value;
                    mesh.Vertices[index] = new Vertex { Coordinates = new Vector3((float)x, (float)y, (float)z), Normal = new Vector3((float)nx, (float)ny, (float)nz) };
                }

                // Then filling the Faces array
                for (var index = 0; index < facesCount; index++)
                {
                    var a = (int)indicesArray[index * 3].Value;
                    var b = (int)indicesArray[index * 3 + 1].Value;
                    var c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender
                var position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            scene.Meshes = meshes.ToArray();

            for (int lightIndex = 0; lightIndex < jsonObject.lights.Count; lightIndex++)
            {
                var light = new Light();
                var x = (float)jsonObject.lights[lightIndex].position[0];
                var y = (float)jsonObject.lights[lightIndex].position[1];
                var z = (float)jsonObject.lights[lightIndex].position[2];
                light.Position = new RawVector3(x, y, z);
                scene.Lights.Add(light);
            }

            for (int cameraIndex = 0; cameraIndex < jsonObject.cameras.Count; cameraIndex++)
            {
                var camera = new Camera();
                var x = (float)jsonObject.cameras[cameraIndex].position[0];
                var y = (float)jsonObject.cameras[cameraIndex].position[1];
                var z = (float)jsonObject.cameras[cameraIndex].position[2];
                var tx = (float)jsonObject.cameras[cameraIndex].rotation[0];
                var ty = (float)jsonObject.cameras[cameraIndex].rotation[1];
                var tz = (float)jsonObject.cameras[cameraIndex].rotation[2];
                camera.Position = new RawVector3(x, y, z);
                camera.Target = new RawVector3(tx, ty, tz);
                scene.Cameras.Add(camera);
            }

            return scene;
        }
    }
}
