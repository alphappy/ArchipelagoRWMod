using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.Archipelago
{
    internal class Settings : OptionInterface
    {
        public static Settings instance = new();
        public static Configurable<bool> boolPrePrimedEchoes = instance.config.Bind("boolPrePrimedEchoes", false, new ConfigurableInfo("Whether Echoes should be automatically primed so that they don't have to be visited twice."));

        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[] { new OpTab(this, "QoL") };
            Vector2 pos = new(120f, 550f);
            List<UIelement> list = new();

            list.Add(new OpLabel(pos, new(400f, 100f), "Quality-of-life settings are client-side and have no effect on logic.\nThe default for each (unchecked) is vanilla behavior."));

            foreach (Configurable<bool> c in new Configurable<bool>[] { boolPrePrimedEchoes })
            {
                list.Add(new OpCheckBox(c, pos));
                list.Add(new OpLabel(pos.x + 35f, pos.y + 3f, c.info.description));
                pos += new Vector2(0f, -35f);
            }

            Tabs[0].AddItems(list.ToArray());
        }
    }
}
