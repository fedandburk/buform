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

        RegisterGroupHeader<ILabeledFormGroup, LabeledFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupLabeledHeaderLayout
        );
        RegisterGroupFooter<ILabeledFormGroup, LabeledFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupLabeledFooterLayout
        );
        RegisterGroupHandler<IListFormGroup, ListFormGroupHandler>();

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
        RegisterItem<IMultilineTextInputFormItem, MultilineTextInputFormViewHolder>(
            Resource.Layout.FormItemMultilineTextInputLayout
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
        where TViewHolder : FormViewHolder<TItem>
    {
        Registry.RegisterItem<TItem, TViewHolder>(resourceId);
    }

    public static void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormHeaderFooterViewHolder<TGroup>
    {
        Registry.RegisterGroupHeader<TGroup, TViewHolder>(resourceId);
    }

    public static void RegisterGroupFooter<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormHeaderFooterViewHolder<TGroup>
    {
        Registry.RegisterGroupFooter<TGroup, TViewHolder>(resourceId);
    }

    public static void RegisterGroupHandler<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            THandler
    >()
        where TGroup : class, IFormGroup
        where THandler : FormGroupHandler<TGroup>
    {
        Registry.RegisterGroupHandler<TGroup, THandler>();
    }

    internal static IFormGroupHandler GetGroupHandler(IFormGroup group)
    {
        return Registry.GetGroupHandler(group);
    }

    internal static bool TryGetItemViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetItemViewType(itemType, out viewType);
    }

    internal static bool TryGetGroupHeaderViewType(Type groupType, out int? viewType)
    {
        return Registry.TryGetGroupHeaderViewType(groupType, out viewType);
    }

    internal static bool TryGetGroupFooterViewType(Type groupType, out int? viewType)
    {
        return Registry.TryGetGroupFooterViewType(groupType, out viewType);
    }

    internal static bool TryGetViewHolderAndResourceId(
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
