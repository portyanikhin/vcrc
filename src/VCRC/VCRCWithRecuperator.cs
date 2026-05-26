// ReSharper disable InconsistentNaming

namespace VCRC;

/// <summary>
/// Single-stage VCRC with recuperator.
/// </summary>
public interface IVCRCWithRecuperator : IVCRC, IHaveRecuperator
{
    /// <summary>
    /// Point 1 – evaporator outlet / recuperator "cold" inlet.
    /// </summary>
    IRefrigerant Point1 { get; }

    /// <summary>
    /// Point 2 – recuperator "cold" outlet / compression stage suction.
    /// </summary>
    IRefrigerant Point2 { get; }

    /// <summary>
    /// Point 3s – isentropic compression stage discharge.
    /// </summary>
    IRefrigerant Point3s { get; }

    /// <summary>
    /// Point 3 – compression stage discharge / condenser or gas cooler inlet.
    /// </summary>
    IRefrigerant Point3 { get; }

    /// <summary>
    /// Point 4 – condenser or gas cooler outlet / recuperator "hot" inlet.
    /// </summary>
    IRefrigerant Point4 { get; }

    /// <summary>
    /// Point 5 – recuperator "hot" outlet / EV inlet.
    /// </summary>
    IRefrigerant Point5 { get; }

    /// <summary>
    /// Point 6 – EV outlet / evaporator inlet.
    /// </summary>
    IRefrigerant Point6 { get; }
}

/// <inheritdoc cref="IVCRCWithRecuperator" />
public class VCRCWithRecuperator : AbstractVCRC, IVCRCWithRecuperator
{
    /// <inheritdoc cref="VCRCWithRecuperator" />
    /// <param name="evaporator">Evaporator.</param>
    /// <param name="recuperator">Recuperator.</param>
    /// <param name="compressor">Compressor.</param>
    /// <param name="heatReleaser">Condenser or gas cooler.</param>
    /// <exception cref="ValidationException">Only one refrigerant should be selected!</exception>
    /// <exception cref="ValidationException">
    /// Condensing temperature should be greater than evaporating temperature!
    /// </exception>
    /// <exception cref="ValidationException">
    /// Too high temperature difference at the recuperator 'hot' side!
    /// </exception>
    public VCRCWithRecuperator(
        IEvaporator evaporator,
        IAuxiliaryHeatExchanger recuperator,
        ICompressor compressor,
        IHeatReleaser heatReleaser
    )
        : base(evaporator, compressor, heatReleaser)
    {
        Recuperator = recuperator;
        new VCRCWithRecuperatorValidator().ValidateAndThrow(this);
        Point2 = Point1.HeatingTo(Point4.Temperature - Recuperator.TemperatureDifference);
        Point3s = Point2.IsentropicCompressionTo(HeatReleaser.Pressure);
        Point3 = Point2.CompressionTo(HeatReleaser.Pressure, Compressor.Efficiency);
        Point5 = Point4.CoolingTo(Point4.Enthalpy - (Point2.Enthalpy - Point1.Enthalpy));
        Point6 = Point5.IsenthalpicExpansionTo(Evaporator.Pressure);
    }

    private IEntropyAnalyzer Analyzer =>
        new EntropyAnalyzer(
            this,
            new EvaporatorNode(EvaporatorSpecificMassFlow, Point6, Point1),
            new HeatReleaserNode(HeatReleaserSpecificMassFlow, Point3s, Point4),
            new EVNode(HeatReleaserSpecificMassFlow, Point5, Point6),
            null,
            null,
            null,
            new RecuperatorNode(
                EvaporatorSpecificMassFlow,
                Point1,
                Point2,
                HeatReleaserSpecificMassFlow,
                Point4,
                Point5
            )
        );

    public IAuxiliaryHeatExchanger Recuperator { get; }
    public IRefrigerant Point1 => Evaporator.Outlet;
    public IRefrigerant Point2 { get; }
    public IRefrigerant Point3s { get; }
    public IRefrigerant Point3 { get; }
    public IRefrigerant Point4 => HeatReleaser.Outlet;
    public IRefrigerant Point5 { get; }
    public IRefrigerant Point6 { get; }
    public sealed override Ratio HeatReleaserSpecificMassFlow { get; } = 100.Percent();

    public sealed override SpecificEnergy IsentropicSpecificWork =>
        Point3s.Enthalpy - Point2.Enthalpy;

    public sealed override SpecificEnergy SpecificCoolingCapacity =>
        Point1.Enthalpy - Point6.Enthalpy;

    public sealed override SpecificEnergy SpecificHeatingCapacity =>
        Point3.Enthalpy - Point4.Enthalpy;

    public override IEntropyAnalysisResult EntropyAnalysis(
        Temperature indoor,
        Temperature outdoor
    ) => Analyzer.PerformAnalysis(indoor, outdoor);
}
