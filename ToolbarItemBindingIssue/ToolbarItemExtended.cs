namespace ToolbarItemBindingIssue
{
    public class ToolbarItemExtended : ToolbarItem
    {
        ContentPage contentPage;

        public ToolbarItemExtended() : base()
        {
            //Task.Run(async () => {
            //    await Task.Delay(50);
            //    OnIsVisibleChanged(this, false, IsVisible);
            //});
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisibleProperty), typeof(bool), typeof(bool), default(bool), propertyChanged: OnIsVisibleChanged);

        public static BindableProperty StartIndexProperty =
            BindableProperty.Create(nameof(StartIndexProperty), typeof(int), typeof(int), -1);

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ToolbarItemExtended item = bindable as ToolbarItemExtended;

            if (item.Parent is ContentPage contPage)
            {
                item.contentPage = contPage;
            }

            if (item.contentPage is not null)
            {
                IList<ToolbarItem> items = item.contentPage.ToolbarItems;

                if ((bool)newvalue && !items.Contains(item))
                {
                    items.Add(item);
                }
                else if (!(bool)newvalue && items.Contains(item))
                {
                    var parent = item.Parent;
                    items.Remove(item);
                    item.Parent = parent;
                }
            }
        }
    }
}

