using Assimp;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XGE3D.Core.SceneSystem
{
    public static class SceneAssembler
    {
        public static SceneData CreateScene(string name)
        {
            SceneData sceneData = new SceneData(name);

            XmlSerializer serializer = new XmlSerializer(typeof(SceneData));

            using (StreamWriter writer = new StreamWriter($"{sceneData.SceneName}.scene"))
            {
                serializer.Serialize(writer, sceneData);
                writer.Close();
            }

            return sceneData;
        }

        public static void AddObject(SceneData sceneData, Entity entity)
        {
            sceneData.AddObject(entity);

            XmlSerializer serializer = new XmlSerializer(typeof(SceneData));

            using (StreamWriter writer = new StreamWriter($"{sceneData.SceneName}.scene"))
            {
                serializer.Serialize(writer, sceneData);
                writer.Close();
            }
        }

        public static void AddObjects(SceneData scene, List<Entity> entities)
        {
            scene.AddObjects(entities);

            XmlSerializer serializer = new XmlSerializer(typeof(SceneData));
            using (StreamWriter writer = new StreamWriter($"{scene.SceneName}.scene"))
            {
                serializer.Serialize(writer, scene);
            }
        }
    }

    public class SceneData
    {
        public string SceneName { get; }
        public List<Entity> Entities{ get; } = new List<Entity>();

        public SceneData()
        {

        }

        public SceneData(string scene_name)
        {
            SceneName = scene_name;
        }

        public void AddObject(Entity entity)
        {
            Entities.Add(entity);
        }

        public void AddObjects(List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                AddObject(entities[i]);
            }
        }
    }
}
