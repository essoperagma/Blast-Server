using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Missiles;
using Game_Core.Obstacles;
using Game_Core.Skills;

namespace Game_Core
{
    public static class TypeIdGenerator
    {
        public static Dictionary<int, IIncomingMessage> incomingMessageIds;
        public static Dictionary<Type, int> outgoingMessageIds;

        public static Dictionary<int, IBlock> blockTypesByIds;
        public static Dictionary<Type, int> idsOfBlocks;

        public static Dictionary<int, Skill> skillTypesByIds;
        public static Dictionary<Type, int> idsOfSkills;

        public static Dictionary<int, Missile> missilesTypesByIds;
        public static Dictionary<Type, int> idsOfMissiles;

        public static Dictionary<int, Obstacle> obstacleTypesByIds;
        public static Dictionary<Type, int> idsOfObstacles;

        static TypeIdGenerator()
        {
            incomingMessageIds = generateIdClassHashMap<IIncomingMessage>();
            outgoingMessageIds = generateTypeIdHashMap(typeof(IOutgoingMessage));

            blockTypesByIds = generateIdClassHashMap<IBlock>();
            idsOfBlocks = generateTypeIdHashMap(typeof (IBlock));

            skillTypesByIds = generateIdClassHashMap<Skill>();
            idsOfSkills = generateTypeIdHashMap(typeof(Skill));

            missilesTypesByIds = generateIdClassHashMap<Missile>();
            idsOfMissiles = generateTypeIdHashMap(typeof(Missile));

            obstacleTypesByIds = generateIdClassHashMap<Obstacle>();
            idsOfObstacles= generateTypeIdHashMap(typeof(Obstacle));
        }

        private static Dictionary<int, T> generateIdClassHashMap<T>()
        {
            Dictionary<int, T> resultDictionary = new Dictionary<int, T>();
            Type type = typeof(T);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);


            KeyValuePair<string, Type>[] nameTypePairs = new KeyValuePair<string, Type>[types.Count()];

            // and put them in a static dictionary.
            int index = 0;
            foreach (Type t in types)
                nameTypePairs[index++] = new KeyValuePair<string, Type>(t.Name, t);

            Array.Sort(nameTypePairs, keyComparison);

            KeyValuePair<string, Type> pair;

            for (int i = 0; i < nameTypePairs.Count(); i++)
            {
                pair = nameTypePairs[i];

                // check uniqueness of the typeId
                T msg = (T)Activator.CreateInstance(pair.Value);
                resultDictionary.Add(i, msg);

            }
            return resultDictionary;
        }

        private static int keyComparison(KeyValuePair<string, Type> keyValuePair, KeyValuePair<string, Type> valuePair)
        {
            return String.Compare(keyValuePair.Key, valuePair.Key, System.StringComparison.Ordinal);
        }

        private static Dictionary<Type, int> generateTypeIdHashMap(Type type)
        {
            Dictionary<Type, int> resultDictionary = new Dictionary<Type, int>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);


            KeyValuePair<string, Type>[] nameTypePairs = new KeyValuePair<string, Type>[types.Count()];

            // and put them in a static dictionary.
            int index = 0;
            foreach (Type t in types)
                nameTypePairs[index++] = new KeyValuePair<string, Type>(t.Name, t);

            Array.Sort(nameTypePairs, keyComparison);

            KeyValuePair<string, Type> pair;

            for (int i = 0; i < nameTypePairs.Count(); i++)
            {
                pair = nameTypePairs[i];

                resultDictionary.Add(pair.Value, i);
            }
            return resultDictionary;
        }
    }
}
