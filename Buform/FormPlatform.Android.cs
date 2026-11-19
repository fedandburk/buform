using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Android.Views;

namespace Buform;

public static class FormPlatform
{
    private static readonly FormRegistry Registry;

    static FormPlatform()
    {
        Registry = new FormRegistry();

        RegisterGroupHeader<TextFormGroup, LabeledFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupTextHeaderLayout
        );
        RegisterGroupFooter<TextFormGroup, LabeledFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupTextFooterLayout
        );
        RegisterGroupHeader<IListFormGroup, LabeledFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupTextHeaderLayout
        );
        RegisterGroupFooter<IListFormGroup, LabeledFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupTextFooterLayout
        );

        RegisterItem<ButtonFormItem, ButtonFormViewHolder>(Resource.Layout.FormItemButtonLayout);
        RegisterItem<DateTimeFormItem, DateTimeFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
        RegisterItem<IAsyncPickerFormItem, AsyncPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
        RegisterItem<ICallbackPickerFormItem, CallbackPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
        RegisterItem<IMultiValuePickerFormItem, MultiValuePickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
        RegisterItem<IPickerFormItem, PopUpPickerFormViewHolder>(
            Resource.Layout.FormItemPickerLayout
        );
        RegisterItem<ISegmentsFormItem, SegmentsFormViewHolder>(
            Resource.Layout.FormItemSegmentsLayout
        );
        RegisterItem<SliderFormItem, SliderFormViewHolder>(Resource.Layout.FormItemSliderLayout);
        RegisterItem<StepperFormItem, StepperFormViewHolder>(Resource.Layout.FormItemStepperLayout);
        RegisterItem<SwitchFormItem, SwitchFormViewHolder>(Resource.Layout.FormItemSwitchLayout);
        RegisterItem<IMultilineTextInputFormItem, TextMultilineFormViewHolder>(
            Resource.Layout.FormItemTextMultilineLayout
        );
        RegisterItem<ITextInputFormItem, TextInputFormViewHolder>(
            Resource.Layout.FormItemTextInputLayout
        );
        RegisterItem<ITextFormItem, TextFormViewHolder>(Resource.Layout.FormItemTextLayout);
    }

    public static void RegisterItem<
        TItem,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TItem : class, IFormItem
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterItem<TItem, TViewHolder>(resourceId);
    }

    public static void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterGroupHeader<TGroup, TViewHolder>(resourceId);
    }

    public static void RegisterGroupFooter<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterGroupFooter<TGroup, TViewHolder>(resourceId);
    }

    public static bool TryGetItemViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetItemViewType(itemType, out viewType);
    }

    public static bool TryGetGroupHeaderViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetGroupHeaderViewType(itemType, out viewType);
    }

    public static bool TryGetGroupFooterViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetGroupFooterViewType(itemType, out viewType);
    }

    public static bool TryGetViewHolderAndResourceId(
        int viewType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            out Type? viewHolderType,
        out int? resourceId
    )
    {
        return Registry.TryGetViewHolderAndResourceId(viewType, out viewHolderType, out resourceId);
    }

    internal static FormViewHolder CreateViewHolder(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            Type viewHolderType,
        View view
    )
    {
        ArgumentNullException.ThrowIfNull(viewHolderType);
        ArgumentNullException.ThrowIfNull(view);

        try
        {
            return (FormViewHolder)Activator.CreateInstance(viewHolderType, view)!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();

            throw;
        }
    }
}
