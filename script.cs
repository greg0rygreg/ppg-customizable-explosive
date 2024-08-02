using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// maybe its not that bad after all

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
                    DescriptionOverride = "General purpose bomb that has been reprogrammed to be as customizable as possible.", //new item description
                    CategoryOverride = ModAPI.FindCategory("Explosives"), //new item category
                    ThumbnailOverride = ModAPI.LoadSprite("newerView.png"), //new item thumbnail (relative path)
                    AfterSpawn = (Instance) => //all code in the AfterSpawn delegate will be executed when the item is spawned
                    {
                        //get the SpriteRenderer and replace its sprite with a custom one
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("why.png");
                        Instance.FixColliders();
                        var phys = Instance.GetComponent<PhysicalBehaviour>();
                        phys.TrueInitialMass = 5f;
                        phys.InitialMass = 5f;
                        phys.rigidbody.mass = 5f;
                        phys.Properties = ModAPI.FindPhysicalProperties("Metal");
                        Instance.AddComponent<cusBombBehaviour>();
                        Instance.AddComponent<cusBombSpriteBeh>();
                    }
                }
            );
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Brick"), //item to derive from
                    NameOverride = "Customizable Explosive (old sprite)", //new item name with a suffix to assure it is globally unique
                    NameToOrderByOverride = "!Customizable Explosive2",
                    DescriptionOverride = "Like a customizable explosive, but with the old sprite", //new item description
                    CategoryOverride = ModAPI.FindCategory("Explosives"), //new item category
                    ThumbnailOverride = ModAPI.LoadSprite("customBombView.png"), //new item thumbnail (relative path)
                    AfterSpawn = (Instance) => //all code in the AfterSpawn delegate will be executed when the item is spawned
                    {
                        //get the SpriteRenderer and replace its sprite with a custom one
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("customBomb.png");
                        Instance.FixColliders();
                        var phys = Instance.GetComponent<PhysicalBehaviour>();
                        phys.TrueInitialMass = 2.5f;
                        phys.InitialMass = 2.5f;
                        phys.rigidbody.mass = 2.5f;
                        phys.Properties = ModAPI.FindPhysicalProperties("Soft");
                        Instance.AddComponent<cusBombBehaviour>();
                    }
                }
            );
        }
    }
    public class cusBombBehaviour : MonoBehaviour
    {
        public float dismemberChance = 0.1f;
        public int fragForce = 8;
        public int rang = 10;
        public bool largeExplosion = true;
        public bool isIndustructable = false;
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
                                    LargeExplosionParticles = largeExplosion,
                                    
                                    //The chance that limbs are torn off (0 - 1, 1 meaning all limbs and 0 meaning none)
                                    DismemberChance = dismemberChance,

                                    //The amount of force for each "fragment" of the explosion. 8 is a pretty powerful explosion. 2 is a regular explosion.
                                    FragmentForce = fragForce,

                                    //The amount of rays cast to simulate fragments. More rays is more lag but higher precision
                                    FragmentationRayCount = 32,

                                    //The ultimate range of the explosion
                                    Range = rang
                                });
                                if (dismemberChance == 0.42f && fragForce == 420 && rang == 42)
                                {
                                    ModAPI.Notify("<color=green>WEED</color>");
                                }
                                if (!isIndustructable)
                                {
                                    Destroy(gameObject);
                                }
                        };
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("printBtn", "Print info", "Print information of the explosive as a notification", () =>
                            {
                                ModAPI.Notify("Dismember chance: " + dismemberChance + " - Fragment force: " + fragForce + " - Range: " + rang + " - Large: " + largeExplosion + " - Indestructability: " + isIndustructable);
                            })
                        );
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
                            new ContextMenuButton("largeToggle", "Toggle large explosion", "Should the particles, if created, be that of a large explosion?", () =>
                            {
                                if (largeExplosion == true)
                                {
                                    largeExplosion = false;
                                    ModAPI.Notify("<color=green>Large explosion set to false</color>");
                                } else
                                {
                                    largeExplosion = true;
                                    ModAPI.Notify("<color=green>Large explosion set to true</color>");
                                }
                                
                            })
                        );
                        GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(
                            new ContextMenuButton("destroyToggle", "Toggle indestructability", "Toggle indestructability", () =>
                            {
                                if (isIndustructable == true)
                                {
                                    isIndustructable = false;
                                    ModAPI.Notify("<color=green>Indestructability set to false</color>");
                                } else
                                {
                                    isIndustructable = true;
                                    ModAPI.Notify("<color=green>Indestructability set to true</color>");
                                }
                                
                            })
                        );
        }
    }
    public class cusBombSpriteBeh : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        private List<Sprite> sprites = new List<Sprite>();
        private int currentSpriteIndex = 0;
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            sprites.Add(ModAPI.LoadSprite("why.png"));
            sprites.Add(ModAPI.LoadSprite("why2.png"));

            StartCoroutine(spriteChange());
        }
        IEnumerator spriteChange()
        {
            while (true)
            {
                currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Count;
                spriteRenderer.sprite = sprites[currentSpriteIndex];

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}