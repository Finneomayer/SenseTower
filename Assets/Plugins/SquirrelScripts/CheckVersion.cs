using Squirrel.Sources;
using Squirrel;
using System.IO;
using TMPro;
using UnityEngine;
using System.Linq;

public class CheckVersion : MonoBehaviour
{
    public TMP_Text myText;
    async void Start()
    {
        using var mgr = new UpdateManager(new SimpleFileSource(new DirectoryInfo("C:\\Release\\")));

        if (mgr.IsInstalledApp)
        {
            var updates = await mgr.CheckForUpdate();
            if (updates.ReleasesToApply.Any())
            {
                myText.text = $"There is new version - {updates.FutureReleaseEntry.Version}";
            }
            else
            {
                myText.text = $"Latest version already installed - {mgr.CurrentlyInstalledVersion()}";
            }
        }
        else
            myText.text = "Nope. App isn't installed.";
    }
}
