using System;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace ToolbarItemBindingIssue.Platforms.Android.Renderers
{
	public class CustomSearchBarRenderer : SearchBarRenderer
    {
		public CustomSearchBarRenderer(Context ctx) : base(ctx)
        {
		}
	}
}

