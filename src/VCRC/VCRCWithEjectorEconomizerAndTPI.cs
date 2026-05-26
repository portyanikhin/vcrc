// ReSharper disable InconsistentNaming

namespace VCRC;

/// <summary>
/// Two-stage VCRC with an ejector as an expansion device,
/// economizer and two-phase injection into the compressor.
/// </summary>
public interface IVCRCWithEjectorEconomizerAndTPI
    : ITwoStageVCRC,
        IHaveEjector,
        IHaveEconomizerWithTPI
{
    /// <summary>
    /// Point 1 – separator vapor outlet / first compression stage suction.
    /// </summary>
    IRefrigerant Point1 { get; }

    /// <summary>
    /// Point 2s – first isentropic compression stage discharge.
    /// </summary>
    IRefrigerant Point2s { get; }

    /// <summary>
    /// Point 2 – first compression stage discharge.
    /// </summary>
    IRefrigerant Point2 { get; }

    /// <summary>
    /// Point 3 – second compression stage suction.
    /// </summary>
    IRefrigerant Point3 { get; }

    /// <summary>
    /// Point 4s – second isentropic compression stage discharge.
    /// </summary>
    IRefrigerant Point4s { get; }

    /// <summary>
    /// Point 4 – second compression stage discharge / condenser or gas cooler inlet.
    /// </summary>
    IRefrigerant Point4 { get; }

    /// <summary>
    /// Point 5 – condenser or gas cooler outlet / first EV inlet / economizer "hot" inlet.
    /// </summary>
    IRefrigerant Point5 { get; }

    /// <summary>
    /// Point 6 – first EV outlet / economizer "cold" inlet.
    /// </summary>
    IRefrigerant Point6 { get; }

    /// <summary>
    /// Point 7 – economizer "cold" outlet /
    /// injection of two-phase IRefrigerant into the compressor.
    /// </summary>
    IRefrigerant Point7 { get; }

    /// <summary>
    /// Point 8 – economizer "hot" outlet / ejector nozzle inlet.
    /// </summary>
    IRefrigerant Point8 { get; }

    /// <summary>
    /// Point 9 – ejector nozzle outlet.
    /// </summary>
    IRefrigerant Point9 { get; }

    /// <summary>
    /// Point 10 – ejector mixing section inlet.
    /// </summary>
    IRefrigerant Point10 { get; }

    /// <summary>
    /// Point 11 – ejector diffuser outlet / separator inlet.
    /// </summary>
    IRefrigerant Point11 { get; }

    /// <summary>
    /// Point 12 – separator liquid outlet / second EV inlet.
    /// </summary>
    IRefrigerant Point12 { get; }

    /// <summary>
    /// Point 13 – second EV outlet / evaporator inlet.
    /// </summary>
    IRefrigerant Point13 { get; }

    /// <summary>
    /// Point 14 – evaporator outlet / ejector suction section inlet.
    /// </summary>
    IRefrigerant Point14 { get; }

    /// <summary>
    /// Point 15 – ejector suction section outlet.
    /// </summary>
    IRefrigerant Point15 { get; }
}

/// <inheritdoc cref="IVCRCWithEjectorEconomizerAndTPI" />
public class VCRCWithEjectorEconomizerAndTPI
    : AbstractTwoStageVCRC,
        IVCRCWithEjectorEconomizerAndTPI
{
    private Pressure _diffuserOutletPressure;
    private IEjectorFlows _ejectorFlows = default!;

    /// <inheritdoc cref="VCRCWithEjectorEconomizerAndTPI" />
    /// <param name="evaporator">Evaporator.</param>
    /// <param name="compressor">Compressor.</param>
    /// <param name="heatReleaser">Condenser or gas cooler.</param>
    /// <param name="ejector">Ejector.</param>
    /// <param name="economizer">Economizer.</param>
    /// <exception cref="ValidationException">Only one refrigerant should be selected!</exception>
    /// <exception cref="ValidationException">
    /// Condensing temperature should be greater than evaporating temperature!
    /// </exception>
    /// <exception cref="ValidationException">
    /// Refrigerant should be a single component or an azeotropic blend!
    /// </exception>
    /// <exception cref="ValidationException">
    /// Too high temperature difference at the economizer 'cold' side!
    /// </exception>
    public VCRCWithEjectorEconomizerAndTPI(
        IEvaporator evaporator,
        ICompressor compressor,
        IHeatReleaser heatReleaser,
        IEjector ejector,
        IAuxiliaryHeatExchanger economizer
    )
        : base(evaporator, compressor, heatReleaser)
    {
        new RefrigerantTypeValidator().ValidateAndThrow(Refrigerant);
        Ejector = ejector;
        Economizer = economizer;
        CalculateDiffuserOutletPressure();
        Point4s = Point3.IsentropicCompressionTo(HeatReleaser.Pressure);
        Point4 = Point3.CompressionTo(HeatReleaser.Pressure, Compressor.Efficiency);
        Point12 = Refrigerant.BubblePointAt(_ejectorFlows.DiffuserOutlet.Pressure);
        Point13 = Point12.IsenthalpicExpansionTo(Evaporator.Pressure);
    }

    private IEntropyAnalyzer Analyzer =>
        new EntropyAnalyzer(
            this,
            new EvaporatorNode(EvaporatorSpecificMassFlow, Point13, Point14),
            new HeatReleaserNode(HeatReleaserSpecificMassFlow, Point4s, Point5),
            new EVNode(IntermediateSpecificMassFlow, Point5, Point6),
            new EVNode(EvaporatorSpecificMassFlow, Point12, Point13),
            null,
            new EjectorNode(
                Point11,
                HeatReleaserSpecificMassFlow - IntermediateSpecificMassFlow,
                Point8,
                EvaporatorSpecificMassFlow,
                Point14
            ),
            null,
            new EconomizerNode(
                IntermediateSpecificMassFlow,
                Point6,
                Point7,
                HeatReleaserSpecificMassFlow - IntermediateSpecificMassFlow,
                Point5,
                Point8
            ),
            new MixingNode(
                Point3,
                HeatReleaserSpecificMassFlow - IntermediateSpecificMassFlow,
                Point2,
                IntermediateSpecificMassFlow,
                Point7
            )
        );

    public IEjector Ejector { get; }
    public IAuxiliaryHeatExchanger Economizer { get; }
    public IRefrigerant Point1 { get; private set; } = default!;
    public IRefrigerant Point2s { get; private set; } = default!;
    public IRefrigerant Point2 { get; private set; } = default!;
    public IRefrigerant Point3 { get; private set; } = default!;
    public IRefrigerant Point4s { get; }
    public IRefrigerant Point4 { get; }
    public IRefrigerant Point5 => HeatReleaser.Outlet;
    public IRefrigerant Point6 { get; private set; } = default!;
    public IRefrigerant Point7 { get; private set; } = default!;
    public IRefrigerant Point8 { get; private set; } = default!;
    public IRefrigerant Point9 => _ejectorFlows.NozzleOutlet;
    public IRefrigerant Point10 => _ejectorFlows.MixingInlet;
    public IRefrigerant Point11 => _ejectorFlows.DiffuserOutlet;
    public IRefrigerant Point12 { get; }
    public IRefrigerant Point13 { get; }
    public IRefrigerant Point14 => Evaporator.Outlet;
    public IRefrigerant Point15 => _ejectorFlows.SuctionOutlet;

    public sealed override Pressure IntermediatePressure =>
        CalculateIntermediatePressure(_diffuserOutletPressure, HeatReleaser.Pressure);

    public sealed override Ratio IntermediateSpecificMassFlow =>
        HeatReleaserSpecificMassFlow
        - EvaporatorSpecificMassFlow
            * (
                Point11.Quality!.Value.DecimalFractions
                / (1 - Point11.Quality!.Value.DecimalFractions)
            );

    public sealed override Ratio HeatReleaserSpecificMassFlow =>
        EvaporatorSpecificMassFlow
        * (Point11.Quality!.Value.DecimalFractions / (1 - Point11.Quality!.Value.DecimalFractions))
        * (1 + (Point2.Enthalpy - Point3.Enthalpy) / (Point3.Enthalpy - Point7.Enthalpy));

    public sealed override SpecificEnergy IsentropicSpecificWork =>
        (HeatReleaserSpecificMassFlow - IntermediateSpecificMassFlow).DecimalFractions
            * (Point2s.Enthalpy - Point1.Enthalpy)
        + HeatReleaserSpecificMassFlow.DecimalFractions * (Point4s.Enthalpy - Point3.Enthalpy);

    public sealed override SpecificEnergy SpecificCoolingCapacity =>
        Point14.Enthalpy - Point13.Enthalpy;

    public sealed override SpecificEnergy SpecificHeatingCapacity =>
        HeatReleaserSpecificMassFlow.DecimalFractions * (Point4.Enthalpy - Point5.Enthalpy);

    public override IEntropyAnalysisResult EntropyAnalysis(
        Temperature indoor,
        Temperature outdoor
    ) => Analyzer.PerformAnalysis(indoor, outdoor);

    private void CalculateDiffuserOutletPressure()
    {
        NewtonRaphson.FindRootNearGuess(
            ToSolve,
            Differentiate.FirstDerivativeFunc(ToSolve),
            Evaporator.Pressure.Pascals + 100,
            Evaporator.Pressure.Pascals + 1,
            HeatReleaser.Pressure.Pascals - 1,
            10
        );
        return;

        double ToSolve(double diffuserOutletPressure)
        {
            _diffuserOutletPressure = diffuserOutletPressure.Pascals();
            Point1 = Refrigerant.DewPointAt(_diffuserOutletPressure);
            Point2s = Point1.IsentropicCompressionTo(IntermediatePressure);
            Point2 = Point1.CompressionTo(IntermediatePressure, Compressor.Efficiency);
            Point3 = Refrigerant.DewPointAt(IntermediatePressure);
            Point6 = Point5.IsenthalpicExpansionTo(IntermediatePressure);
            new VCRCWithEjectorEconomizerAndTPIValidator().ValidateAndThrow(this);
            Point8 = Point5.CoolingTo(Point6.Temperature + Economizer.TemperatureDifference);
            Point7 = Point6.HeatingTo(
                (
                    (
                        Point6.Enthalpy.JoulesPerKilogram
                            * (
                                Point2.Enthalpy.JoulesPerKilogram
                                - Point3.Enthalpy.JoulesPerKilogram
                            )
                        + Point3.Enthalpy.JoulesPerKilogram
                            * (
                                Point5.Enthalpy.JoulesPerKilogram
                                - Point8.Enthalpy.JoulesPerKilogram
                            )
                    )
                    / (
                        Point2.Enthalpy.JoulesPerKilogram
                        - Point3.Enthalpy.JoulesPerKilogram
                        + Point5.Enthalpy.JoulesPerKilogram
                        - Point8.Enthalpy.JoulesPerKilogram
                    )
                ).JoulesPerKilogram()
            );
            _ejectorFlows = Ejector.CalculateFlows(Point8, Point14);
            return (_ejectorFlows.DiffuserOutlet.Pressure - _diffuserOutletPressure).Pascals;
        }
    }
}
