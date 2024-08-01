using UnityEngine;
using UnityEngine.Events;

// this is my first ever useful mod ive made dont laugh at me

namespace Mod
{
    public class Mod
    {
        
        public static void Main()
        {
            
            // register item to the mod api
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Brick"), //item to derive from
                    NameOverride = "Customizable Explosive", //new item name with a suffix to assure it is globally unique
                    NameToOrderByOverride = "!Customizable Explosive",
                    DescriptionOverride = "Explosive with custom properties that you can set before detonating.", //new item description
                    CategoryOverride = ModAPI.FindCategory("Explosives"), //new item category
                    ThumbnailOverride = ModAPI.LoadSprite("customBombView.png"), //new item thumbnail (relative path)
                    AfterSpawn = (Instance) => //all code in the AfterSpawn delegate will be executed when the item is spawned
                    {
                        //get the SpriteRenderer and replace its sprite with a custom one
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("customBomb.png");
                        Instance.FixColliders();
                        var phys = Instance.GetComponent<PhysicalBehaviour>();
                        phys.Properties = ModAPI.FindPhysicalProperties("Flammable metal");
                        Instance.AddComponent<bombBehaviour>();
                    }
                }
            );
        }
    }
    public class bombBehaviour : MonoBehaviour
    {
        public float dismemberChance = 0.1f;
        public int fragForce = 8;
        public int rang = 10;
        private void Start()
        {
                        gameObject.AddComponent<UseEventTrigger>().Action = () => {
                                ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters
                                {
                                    //Explosion center
                                    Position = transform.position,

                                    //Should particles be created and sound played? 
                                    CreateParticlesAndSound = true,

                                    //Should the particles, if created, be that of a large explosion?
                                    LargeExplosionParticles = true,
                                    
                                    //The chance that limbs are torn off (0 - 1, 1 meaning all limbs and 0 meaning none)
                                    DismemberChance = dismemberChance,

                                    //The amount of force for each "fragment" of the explosion. 8 is a pretty powerful explosion. 2 is a regular explosion.
                                    FragmentForce = fragForce,

                                    //The amount of rays cast to simulate fragments. More rays is more lag but higher precision
                                    FragmentationRayCount = 32,

                                    //The ultimate range of the explosion
                                    Range = rang
                                });
                                Destroy(gameObject);
                        };
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("dismemberChanceBtn", "Set dismemberment chance", "The chance that limbs are torn off (0 - 1, 1 meaning all limbs and 0 meaning none)", () =>
                            {
                                DialogBox dialog = (DialogBox)null;
                                dialog = DialogBoxManager.TextEntry("Enter the dismemberment chance\n<color=green><size=28>Current: "+dismemberChance, "Number",
                                    new DialogButton("Apply", true, new UnityAction[1]
                                    {
                                        (UnityAction)(() => {
                                            if(dialog.EnteredText != "")
                                            {
                                                dismemberChance = float.Parse(dialog.EnteredText);
                                                ModAPI.Notify("<color=green> Chance set to " + dialog.EnteredText + "</color>");
                                            }
                                            else
                                            {
                                                ModAPI.Notify("<color=red>You have not entered a number!</color>");
                                            }
                                        })
                                    }),
                                    new DialogButton("Cancel", true, (UnityAction)(() => dialog.Close()))
                                );
                            })
                        );
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("fragForceBtn", "Set fragment force", "The amount of force for each \"fragment\" of the explosion. 8 is a pretty powerful explosion. 2 is a regular explosion.", () =>
                            {
                                DialogBox dialog = (DialogBox)null;
                                dialog = DialogBoxManager.TextEntry("Enter the fragment force\n<color=green><size=28>Current: "+fragForce, "Number",
                                    new DialogButton("Apply", true, new UnityAction[1]
                                    {
                                        (UnityAction)(() => {
                                            if(dialog.EnteredText != "")
                                            {
                                                fragForce = int.Parse(dialog.EnteredText);
                                                ModAPI.Notify("<color=green> Force set to " + dialog.EnteredText + "</color>");
                                            }
                                            else
                                            {
                                                ModAPI.Notify("<color=red>You have not entered a number!</color>");
                                            }
                                        })
                                    }),
                                    new DialogButton("Cancel", true, (UnityAction)(() => dialog.Close()))
                                );
                            })
                        );
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("rangeBtn", "Set range", "The ultimate range of the explosion.", () =>
                            {
                                DialogBox dialog = (DialogBox)null;
                                dialog = DialogBoxManager.TextEntry("Enter the range\n<color=green><size=28>Current: "+rang, "Number",
                                    new DialogButton("Apply", true, new UnityAction[1]
                                    {
                                        (UnityAction)(() => {
                                            if(dialog.EnteredText != "")
                                            {
                                                rang = int.Parse(dialog.EnteredText);
                                                ModAPI.Notify("<color=green> Range set to " + dialog.EnteredText + "</color>");
                                            }
                                            else
                                            {
                                                ModAPI.Notify("<color=red>You have not entered a number!</color>");
                                            }
                                        })
                                    }),
                                    new DialogButton("Cancel", true, (UnityAction)(() => dialog.Close()))
                                );
                            })
                        );
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("printBtn", "Print info", "Print information of the explosive as a notification", () =>
                            {
                                ModAPI.Notify("Dismember chance: " + dismemberChance + " - Fragment force: " + fragForce + " - Range: " + rang);
                            })
                        );
        }
    }
}
