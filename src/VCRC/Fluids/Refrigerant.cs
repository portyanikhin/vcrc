using UnitsNet.NumberExtensions.NumberToTemperatureDelta;

namespace VCRC;

/// <summary>
/// VCRC working fluid.
/// </summary>
public interface IRefrigerant : IFluid
{
    /// <summary>
    /// Absolute pressure at the critical point (by default, kPa).
    /// </summary>
    /// <exception cref="NullReferenceException">Invalid critical pressure!</exception>
    new Pressure CriticalPressure { get; }

    /// <summary>
    /// Temperature at the critical point (by default, °C).
    /// </summary>
    /// <exception cref="NullReferenceException">Invalid critical temperature!</exception>
    new Temperature CriticalTemperature { get; }

    /// <summary>
    /// Absolute pressure at the triple point (by default, kPa).
    /// </summary>
    /// <exception cref="NullReferenceException">Invalid triple pressure!</exception>
    new Pressure TriplePressure { get; }

    /// <summary>
    /// Temperature at the triple point (by default, °C).
    /// </summary>
    /// <exception cref="NullReferenceException">Invalid triple temperature!</exception>
    new Temperature TripleTemperature { get; }

    /// <summary>
    /// Temperature glide at atmospheric pressure (by default, K).
    /// </summary>
    TemperatureDelta Glide { get; }

    /// <summary>
    /// <c>true</c> if the refrigerant has a temperature glide.
    /// </summary>
    bool HasGlide { get; }

    /// <summary>
    /// <c>true</c> if the refrigerant is a single component.
    /// </summary>
    bool IsSingleComponent { get; }

    /// <summary>
    /// <c>true</c> if the refrigerant is an azeotropic blend.
    /// </summary>
    bool IsAzeotropicBlend { get; }

    /// <summary>
    /// <c>true</c> if the refrigerant is a zeotropic blend.
    /// </summary>
    bool IsZeotropicBlend { get; }

    /// <summary>
    /// Subcooled refrigerant.
    /// </summary>
    /// <param name="bubblePointTemperature">Bubble point temperature.</param>
    /// <param name="subcooling">Subcooling.</param>
    /// <returns>Subcooled refrigerant.</returns>
    /// <exception cref="ArgumentException">Invalid subcooling!</exception>
    IRefrigerant Subcooled(Temperature bubblePointTemperature, TemperatureDelta subcooling);

    /// <summary>
    /// Subcooled refrigerant.
    /// </summary>
    /// <param name="pressure">Pressure.</param>
    /// <param name="subcooling">Subcooling.</param>
    /// <returns>Subcooled refrigerant.</returns>
    /// <exception cref="ArgumentException">Invalid subcooling!</exception>
    IRefrigerant Subcooled(Pressure pressure, TemperatureDelta subcooling);

    /// <summary>
    /// Superheated refrigerant.
    /// </summary>
    /// <param name="dewPointTemperature">Dew point temperature.</param>
    /// <param name="superheat">Superheat.</param>
    /// <returns>Superheated refrigerant.</returns>
    /// <exception cref="ArgumentException">Invalid superheat!</exception>
    IRefrigerant Superheated(Temperature dewPointTemperature, TemperatureDelta superheat);

    /// <summary>
    /// Superheated refrigerant.
    /// </summary>
    /// <param name="pressure">Pressure.</param>
    /// <param name="superheat">Superheat.</param>
    /// <returns>Superheated refrigerant.</returns>
    /// <exception cref="ArgumentException">Invalid superheat!</exception>
    IRefrigerant Superheated(Pressure pressure, TemperatureDelta superheat);

    /// <inheritdoc cref="IFluid.SpecifyPhase" />
    new IRefrigerant SpecifyPhase(Phases phase);

    /// <inheritdoc cref="IFluid.UnspecifyPhase" />
    new IRefrigerant UnspecifyPhase();

    /// <inheritdoc cref="IFluid.WithState" />
    new IRefrigerant WithState(
        IKeyedInput<Parameters> firstInput,
        IKeyedInput<Parameters> secondInput
    );

    /// <inheritdoc cref="IFluid.IsentropicCompressionTo" />
    new IRefrigerant IsentropicCompressionTo(Pressure pressure);

    /// <inheritdoc cref="IFluid.CompressionTo" />
    new IRefrigerant CompressionTo(Pressure pressure, Ratio isentropicEfficiency);

    /// <inheritdoc cref="IFluid.IsenthalpicExpansionTo" />
    new IRefrigerant IsenthalpicExpansionTo(Pressure pressure);

    /// <inheritdoc cref="IFluid.IsentropicExpansionTo" />
    new IRefrigerant IsentropicExpansionTo(Pressure pressure);

    /// <inheritdoc cref="IFluid.ExpansionTo" />
    new IRefrigerant ExpansionTo(Pressure pressure, Ratio isentropicEfficiency);

    /// <inheritdoc cref="IFluid.CoolingTo(Temperature, Pressure?)" />
    new IRefrigerant CoolingTo(Temperature temperature, Pressure? pressureDrop = null);

    /// <inheritdoc cref="IFluid.CoolingTo(SpecificEnergy, Pressure?)" />
    new IRefrigerant CoolingTo(SpecificEnergy enthalpy, Pressure? pressureDrop = null);

    /// <inheritdoc cref="IFluid.HeatingTo(Temperature, Pressure?)" />
    new IRefrigerant HeatingTo(Temperature temperature, Pressure? pressureDrop = null);

    /// <inheritdoc cref="IFluid.HeatingTo(SpecificEnergy, Pressure?)" />
    new IRefrigerant HeatingTo(SpecificEnergy enthalpy, Pressure? pressureDrop = null);

    /// <inheritdoc cref="IFluid.BubblePointAt(Pressure)" />
    new IRefrigerant BubblePointAt(Pressure pressure);

    /// <inheritdoc cref="IFluid.BubblePointAt(Temperature)" />
    new IRefrigerant BubblePointAt(Temperature temperature);

    /// <inheritdoc cref="IFluid.DewPointAt(Pressure)" />
    new IRefrigerant DewPointAt(Pressure pressure);

    /// <inheritdoc cref="IFluid.DewPointAt(Temperature)" />
    new IRefrigerant DewPointAt(Temperature temperature);

    /// <inheritdoc cref="IFluid.TwoPhasePointAt" />
    new IRefrigerant TwoPhasePointAt(Pressure pressure, Ratio quality);

    /// <inheritdoc cref="IFluid.Mixing" />
    IRefrigerant Mixing(
        Ratio firstSpecificMassFlow,
        IRefrigerant first,
        Ratio secondSpecificMassFlow,
        IRefrigerant second
    );

    /// <inheritdoc cref="IClonable{T}.Clone" />
    new IRefrigerant Clone();

    /// <inheritdoc cref="IFactory{T}.Factory" />
    new IRefrigerant Factory();
}

/// <inheritdoc cref="IRefrigerant" />
public class Refrigerant : Fluid, IRefrigerant
{
    /// <inheritdoc cref="Refrigerant" />
    /// <param name="name">Selected refrigerant name.</param>
    /// <exception cref="ValidationException">
    /// The selected fluid is not a refrigerant (its name should start with 'R')!
    /// </exception>
    public Refrigerant(FluidsList name)
        : base(name) => new RefrigerantValidator().ValidateAndThrow(this);

    public new Pressure CriticalPressure =>
        base.CriticalPressure ?? throw new NullReferenceException("Invalid critical pressure!");

    public new Temperature CriticalTemperature =>
        base.CriticalTemperature
        ?? throw new NullReferenceException("Invalid critical temperature!");

    public new Pressure TriplePressure =>
        base.TriplePressure ?? throw new NullReferenceException("Invalid triple pressure!");

    public new Temperature TripleTemperature =>
        base.TripleTemperature ?? throw new NullReferenceException("Invalid triple temperature!");

    public TemperatureDelta Glide =>
        (DewPointAt(1.Atmospheres()).Temperature - BubblePointAt(1.Atmospheres()).Temperature)
            .Abs()
            .ToUnit(TemperatureDeltaUnit.Kelvin);

    public bool HasGlide => Glide > 0.01.Kelvins();
    public bool IsSingleComponent => !IsAzeotropicBlend && !IsZeotropicBlend;
    public bool IsAzeotropicBlend => BlendRegex(false).IsMatch(Name.ToString());
    public bool IsZeotropicBlend => BlendRegex(true).IsMatch(Name.ToString());

    public IRefrigerant Subcooled(
        Temperature bubblePointTemperature,
        TemperatureDelta subcooling
    ) =>
        subcooling < TemperatureDelta.Zero ? throw new ArgumentException("Invalid subcooling!")
        : subcooling.Equals(TemperatureDelta.Zero, Tolerance.Kelvins())
            ? BubblePointAt(bubblePointTemperature)
        : BubblePointAt(bubblePointTemperature).CoolingTo(bubblePointTemperature - subcooling);

    public IRefrigerant Subcooled(Pressure pressure, TemperatureDelta subcooling) =>
        subcooling < TemperatureDelta.Zero ? throw new ArgumentException("Invalid subcooling!")
        : subcooling.Equals(TemperatureDelta.Zero, Tolerance.Kelvins()) ? BubblePointAt(pressure)
        : BubblePointAt(pressure).CoolingTo(BubblePointAt(pressure).Temperature - subcooling);

    public IRefrigerant Superheated(Temperature dewPointTemperature, TemperatureDelta superheat) =>
        superheat < TemperatureDelta.Zero ? throw new ArgumentException("Invalid superheat!")
        : superheat.Equals(TemperatureDelta.Zero, Tolerance.Kelvins())
            ? DewPointAt(dewPointTemperature)
        : DewPointAt(dewPointTemperature).HeatingTo(dewPointTemperature + superheat);

    public IRefrigerant Superheated(Pressure pressure, TemperatureDelta superheat) =>
        superheat < TemperatureDelta.Zero ? throw new ArgumentException("Invalid superheat!")
        : superheat.Equals(TemperatureDelta.Zero, Tolerance.Kelvins()) ? DewPointAt(pressure)
        : DewPointAt(pressure).HeatingTo(DewPointAt(pressure).Temperature + superheat);

    public new IRefrigerant SpecifyPhase(Phases phase) => (Refrigerant)base.SpecifyPhase(phase);

    public new IRefrigerant UnspecifyPhase() => (Refrigerant)base.UnspecifyPhase();

    public new IRefrigerant WithState(
        IKeyedInput<Parameters> firstInput,
        IKeyedInput<Parameters> secondInput
    ) => (Refrigerant)base.WithState(firstInput, secondInput);

    public new IRefrigerant IsentropicCompressionTo(Pressure pressure) =>
        (Refrigerant)base.IsentropicCompressionTo(pressure);

    public new IRefrigerant CompressionTo(Pressure pressure, Ratio isentropicEfficiency) =>
        (Refrigerant)base.CompressionTo(pressure, isentropicEfficiency);

    public new IRefrigerant IsenthalpicExpansionTo(Pressure pressure) =>
        (Refrigerant)base.IsenthalpicExpansionTo(pressure);

    public new IRefrigerant IsentropicExpansionTo(Pressure pressure) =>
        (Refrigerant)base.IsentropicExpansionTo(pressure);

    public new IRefrigerant ExpansionTo(Pressure pressure, Ratio isentropicEfficiency) =>
        (Refrigerant)base.ExpansionTo(pressure, isentropicEfficiency);

    public new IRefrigerant CoolingTo(Temperature temperature, Pressure? pressureDrop = null) =>
        (Refrigerant)base.CoolingTo(temperature, pressureDrop);

    public new IRefrigerant CoolingTo(SpecificEnergy enthalpy, Pressure? pressureDrop = null) =>
        (Refrigerant)base.CoolingTo(enthalpy, pressureDrop);

    public new IRefrigerant HeatingTo(Temperature temperature, Pressure? pressureDrop = null) =>
        (Refrigerant)base.HeatingTo(temperature, pressureDrop);

    public new IRefrigerant HeatingTo(SpecificEnergy enthalpy, Pressure? pressureDrop = null) =>
        (Refrigerant)base.HeatingTo(enthalpy, pressureDrop);

    public new IRefrigerant BubblePointAt(Pressure pressure) =>
        (Refrigerant)base.BubblePointAt(pressure);

    public new IRefrigerant BubblePointAt(Temperature temperature) =>
        (Refrigerant)base.BubblePointAt(temperature);

    public new IRefrigerant DewPointAt(Pressure pressure) => (Refrigerant)base.DewPointAt(pressure);

    public new IRefrigerant DewPointAt(Temperature temperature) =>
        (Refrigerant)base.DewPointAt(temperature);

    public new IRefrigerant TwoPhasePointAt(Pressure pressure, Ratio quality) =>
        (Refrigerant)base.TwoPhasePointAt(pressure, quality);

    public IRefrigerant Mixing(
        Ratio firstSpecificMassFlow,
        IRefrigerant first,
        Ratio secondSpecificMassFlow,
        IRefrigerant second
    ) => (Refrigerant)base.Mixing(firstSpecificMassFlow, first, secondSpecificMassFlow, second);

    public new IRefrigerant Clone() => (Refrigerant)base.Clone();

    public new IRefrigerant Factory() => (Refrigerant)base.Factory();

    protected override AbstractFluid CreateInstance() => new Refrigerant(Name);

    private static Regex BlendRegex(bool zeotropic) =>
        new(zeotropic ? @"^R4\d{2}" : @"^R5\d{2}", RegexOptions.Compiled);
}
