using RoR2.ContentManagement;
using System.Collections;
using UnityEngine;

namespace FixedspawnDissonance
{
    // Token: 0x02000003 RID: 3
    public class Content : IContentPackProvider
    {
        public string identifier
        {
            get
            {
                return "FixedspawnDissonance.content";
            }
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(Content.content, args.output);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            Content.content.bodyPrefabs.Add(new GameObject[]
            {
                FixedspawnDissonance.SoulGreaterWispBody
            });
            Content.content.masterPrefabs.Add(new GameObject[]
            {
                FixedspawnDissonance.SoulGreaterWispMaster
            });
            yield break;
        }

        // Token: 0x04000010 RID: 16
        public static ContentPack content = new ContentPack();
    }
}
