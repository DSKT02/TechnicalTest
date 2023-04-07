public interface I_CustomizableInput<T>
{
    public T CustomInput { get; set; }

    public void CustomInputUpdate();

    public bool CustomInputEnabled();
}
