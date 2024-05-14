using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace Genepack_Fullname
{
    public class HarmonyPatches
    {
        [StaticConstructorOnStartup]
        public static class Startup
        {
            static Startup() => new Harmony("localghost.GenepackFullname").PatchAll();

            [HarmonyPatch(typeof(GeneSet), nameof(GeneSet.Label), MethodType.Getter)]
            public static class GenerateNamePatch
            {
                static void Prefix(GeneSet __instance)
                {
                    var nameinfo = typeof(GeneSet).GetField(
                        "name",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );
                    var geneinfo = typeof(GeneSet).GetField(
                        "genes",
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );
                    List<GeneDef> genes = geneinfo.GetValue(__instance) as List<GeneDef>;
                    var name = GrammarResolver.Resolve(
                        "r_name",
                        new GrammarRequest
                        {
                            Includes = { RulePackDefOf.NamerGenepack },
                            Rules =
                            {
                                new Rule_String(
                                    "geneWord",
                                    genes.Select(gene => gene.LabelShortAdj).ToCommaList()
                                ),
                                new Rule_String("geneCountMinusOne", (genes.Count - 1).ToString())
                            },
                            Constants = { { "geneCount", "1" } }
                        },
                        null,
                        false,
                        null,
                        null,
                        null,
                        false
                    );
                    nameinfo.SetValue(__instance, name);
                }
            }
        }
    }
}
