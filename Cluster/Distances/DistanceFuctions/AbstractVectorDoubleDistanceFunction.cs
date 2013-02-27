using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socona.Clustering.Distances.DistanceFuctions
{
   
/**
 * Abstract base class for the most common family of distance functions: defined
 * on number vectors and returning double values.
 * 
 * @author Erich Schubert
 * 
 * @apiviz.landmark
 * @apiviz.uses NumberVector
 * @apiviz.has DoubleDistance
 */
public abstract class AbstractVectorDoubleDistanceFunction : AbstractPrimitiveDistanceFunction<NumberVector<?, ?>, DoubleDistance> implements PrimitiveDoubleDistanceFunction<NumberVector<?, ?>>, NumberVectorDistanceFunction<DoubleDistance> {
  /**
   * Constructor.
   */
  public AbstractVectorDoubleDistanceFunction() {
    super();
  }

  @Override
  public SimpleTypeInformation<? super NumberVector<?, ?>> getInputTypeRestriction() {
    return TypeUtil.NUMBER_VECTOR_FIELD;
  }

  @Override
  public final DoubleDistance distance(NumberVector<?, ?> o1, NumberVector<?, ?> o2) {
    return new DoubleDistance(doubleDistance(o1, o2));
  }

  @Override
  public DoubleDistance getDistanceFactory() {
    return DoubleDistance.FACTORY;
  }
}
}
