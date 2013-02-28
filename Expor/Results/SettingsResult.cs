using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Results
{

    public class SettingsResult : BasicResult
    {
        /**
         * Settings storage.
         */
        ICollection<IPair<Object, IParameter>> settings;

        /**
         * Constructor.
         * 
         * @param settings Settings to store
         */
        public SettingsResult(ICollection<IPair<Object, IParameter>> settings)
            : base("Settings", "settings")
        {
            this.settings = settings;
        }

        /**
         * Get the settings
         * @return the settings
         */
        public ICollection<IPair<Object, IParameter>> GetSettings()
        {
            return settings;
        }
    }
}
