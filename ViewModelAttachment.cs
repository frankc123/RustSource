using System;
using UnityEngine;

public class ViewModelAttachment : MonoBehaviour
{
    [NonSerialized]
    private ViewModel boundViewModel;
    [SerializeField]
    private SkinnedMeshRenderer[] renderers;

    private void OnDestroy()
    {
        if (this.boundViewModel != null)
        {
            this.boundViewModel.RemoveRenderers(this.renderers);
        }
    }

    public ViewModel viewModel
    {
        get
        {
            return this.boundViewModel;
        }
        set
        {
            if (!object.ReferenceEquals(this.boundViewModel, value))
            {
                if (this.boundViewModel != null)
                {
                    this.boundViewModel.RemoveRenderers(this.renderers);
                    this.boundViewModel = null;
                }
                if (value != null)
                {
                    this.boundViewModel = value;
                    this.boundViewModel.AddRenderers(this.renderers);
                }
            }
        }
    }
}

