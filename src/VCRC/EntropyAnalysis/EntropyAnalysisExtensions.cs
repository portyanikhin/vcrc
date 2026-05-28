namespace VCRC;

/// <summary>
/// Entropy analysis extension methods.
/// </summary>
public static class EntropyAnalysisExtensions
{
    /// <summary>
    /// Performs VCRC entropy analysis over a range of indoor and outdoor temperatures.
    /// </summary>
    /// <param name="cycles">Enumerable of VCRCs.</param>
    /// <param name="indoor">Enumerable of indoor temperatures.</param>
    /// <param name="outdoor">Enumerable of outdoor temperatures.</param>
    /// <returns>Result of the VCRC entropy analysis in range of temperatures.</returns>
    /// <exception cref="ArgumentException">Inputs should have the same length!</exception>
    public static IEntropyAnalysisResult EntropyAnalysis(
        this IEnumerable<IEntropyAnalysable> cycles,
        IEnumerable<Temperature> indoor,
        IEnumerable<Temperature> outdoor
    )
    {
        var cyclesList = cycles.ToList();
        var indoorList = indoor.ToList();
        var outdoorList = outdoor.ToList();
        return cyclesList.Count == indoorList.Count && indoorList.Count == outdoorList.Count
            ? cyclesList
                .Select((cycle, i) => cycle.EntropyAnalysis(indoorList[i], outdoorList[i]))
                .Average()
            : throw new ArgumentException("Inputs should have the same length!");
    }

    /// <summary>
    /// Computes the average of the entropy analysis results.
    /// </summary>
    /// <param name="results">Enumerable of the entropy analysis results.</param>
    /// <returns>The average.</returns>
    public static IEntropyAnalysisResult Average(this IEnumerable<IEntropyAnalysisResult> results)
    {
        var resultsList = results.ToList();
        return new EntropyAnalysisResult(
            resultsList.Average(i => i.ThermodynamicPerfection.Percent).Percent(),
            resultsList.Average(i => i.MinSpecificWorkRatio.Percent).Percent(),
            resultsList.Average(i => i.CompressorEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.CondenserEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.GasCoolerEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.ExpansionValvesEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.EjectorEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.EvaporatorEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.RecuperatorEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.EconomizerEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.MixingEnergyLossRatio.Percent).Percent(),
            resultsList.Average(i => i.AnalysisRelativeError.Percent).Percent()
        );
    }
}
