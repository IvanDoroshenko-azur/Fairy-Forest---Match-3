using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class Spawner : MonoBehaviour
    {
        public List<MatchObject> spawned;
        public MatchObject lastSpawned;
        public GridCell gridCell;

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet GOSet { get { return MBoard.MatchSet; } }

        public bool IsEmpty
        {
            get
            {
                return spawned.Count == 0;
            }
        }

        #region regular
        private void Start()
        {
            spawned = new List<MatchObject>();
        }
        #endregion regular

        /// <summary>
        /// spawn new MO or return previous spawned
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public MatchObject Get()
        {
            MatchObject match = MatchObject.Create(GOSet.GetMainRandomObjects(1)[0],transform.position, false, false, MBoard.TargetCollectEventHandler, MBoard.MatchScoreCollectHandler);
            lastSpawned = match;
            if(match) match.transform.localScale = transform.lossyScale;
            return match;
        }
    }
}