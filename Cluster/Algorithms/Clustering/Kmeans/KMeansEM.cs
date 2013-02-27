﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Log;
namespace Socona.Clustering.Algorithms.Clustering.Kmeans
{
  
public class KMedoidsEM<V, D > 
    :AbstractDistanceBasedAlgorithm<V, D, Clustering<MedoidModel>> , IClusteringAlgorithm<Clustering<MedoidModel>> 
        where D: NumberDistance<D, ?>
{
  /**
   * The logger for this class.
   */
  private static const Logging logger = Logging.GetLogger(typeof(KMedoidsEM));

  /**
   * Holds the value of {@link AbstractKMeans#K_ID}.
   */
  protected int k;

  /**
   * Holds the value of {@link AbstractKMeans#MAXITER_ID}.
   */
  protected int maxiter;

  /**
   * Method to choose initial means.
   */
  protected KMedoidsInitialization<V> initializer;

  /**
   * Constructor.
   * 
   * @param distanceFunction distance function
   * @param k k parameter
   * @param maxiter Maxiter parameter
   * @param initializer Function to generate the initial means
   */
  public KMedoidsEM(PrimitiveDistanceFunction<? super V, D> distanceFunction, int k, int maxiter, KMedoidsInitialization<V> initializer) {
    super(distanceFunction);
    this.k = k;
    this.maxiter = maxiter;
    this.initializer = initializer;
  }

  /**
   * Run k-medoids
   * 
   * @param database Database
   * @param relation relation to use
   * @return result
   */
  public Clustering<MedoidModel> run(Database database, Relation<V> relation) {
    if(relation.size() <= 0) {
      return new Clustering<MedoidModel>("k-Medoids Clustering", "kmedoids-clustering");
    }
    DistanceQuery<V, D> distQ = database.getDistanceQuery(relation, getDistanceFunction());
    // Choose initial medoids
    ArrayModifiableDBIDs medoids = DBIDUtil.newArray(initializer.chooseInitialMedoids(k, distQ));
    // Setup cluster assignment store
    List<ModifiableDBIDs> clusters = new ArrayList<ModifiableDBIDs>();
    for(int i = 0; i < k; i++) {
      clusters.add(DBIDUtil.newHashSet(relation.size() / k));
    }
    Mean[] mdists = Mean.newArray(k);

    // Initial assignment to nearest medoids
    // TODO: reuse this information, from the build phase, when possible?
    assignToNearestCluster(medoids, mdists, clusters, distQ);

    // Swap phase
    boolean changed = true;
    while(changed) {
      changed = false;
      // Try to swap the medoid with a better cluster member:
      for(int i = 0; i < k; i++) {
        DBID med = medoids.get(i);
        DBID best = null;
        Mean bestm = mdists[i];
        for(DBIDIter iter = clusters.get(i).iter(); iter.valid(); iter.advance()) {
          if(med.sameDBID(iter)) {
            continue;
          }
          Mean mdist = new Mean();
          for(DBIDIter iter2 = clusters.get(i).iter(); iter2.valid(); iter2.advance()) {
            mdist.put(distQ.distance(iter, iter2).doubleValue());
          }
          if(mdist.getMean() < bestm.getMean()) {
            best = iter.getDBID();
            bestm = mdist;
          }
        }
        if(best != null && !med.sameDBID(best)) {
          changed = true;
          medoids.set(i, best);
          mdists[i] = bestm;
        }
      }
      // Reassign
      if(changed) {
        assignToNearestCluster(medoids, mdists, clusters, distQ);
      }
    }

    // Wrap result
    Clustering<MedoidModel> result = new Clustering<MedoidModel>("k-Medoids Clustering", "kmedoids-clustering");
    for(int i = 0; i < clusters.size(); i++) {
      MedoidModel model = new MedoidModel(medoids.get(i));
      result.addCluster(new Cluster<MedoidModel>(clusters.get(i), model));
    }
    return result;
  }

  /**
   * Returns a list of clusters. The k<sup>th</sup> cluster contains the ids of
   * those FeatureVectors, that are nearest to the k<sup>th</sup> mean.
   * 
   * @param means a list of k means
   * @param mdist Mean distances
   * @param clusters cluster assignment
   * @param distQ distance query
   * @return true when the object was reassigned
   */
  protected boolean assignToNearestCluster(ArrayDBIDs means, Mean[] mdist, List<? extends ModifiableDBIDs> clusters, DistanceQuery<V, D> distQ) {
    boolean changed = false;

    double[] dists = new double[k];
    for(DBIDIter iditer = distQ.getRelation().iterDBIDs(); iditer.valid(); iditer.advance()) {
      int minIndex = 0;
      double mindist = Double.POSITIVE_INFINITY;
      for(int i = 0; i < k; i++) {
        dists[i] = distQ.distance(iditer, means.get(i)).doubleValue();
        if(dists[i] < mindist) {
          minIndex = i;
          mindist = dists[i];
        }
      }
      if(clusters.get(minIndex).add(iditer)) {
        changed = true;
        mdist[minIndex].put(mindist);
        // Remove from previous cluster
        // TODO: keep a list of cluster assignments to save this search?
        for(int i = 0; i < k; i++) {
          if(i != minIndex) {
            if(clusters.get(i).remove(iditer)) {
              mdist[minIndex].put(dists[i], -1);
              break;
            }
          }
        }
      }
    }
    return changed;
  }

  @Override
  public TypeInformation[] getInputTypeRestriction() {
    return TypeUtil.array(getDistanceFunction().getInputTypeRestriction());
  }

  @Override
  protected Logging getLogger() {
    return logger;
  }

  /**
   * Parameterization class.
   * 
   * @author Erich Schubert
   * 
   * @apiviz.exclude
   */
  public static class Parameterizer<V, D extends NumberDistance<D, ?>> extends AbstractPrimitiveDistanceBasedAlgorithm.Parameterizer<V, D> {
    protected int k;

    protected int maxiter;

    protected KMedoidsInitialization<V> initializer;

    @Override
    protected void makeOptions(Parameterization config) {
      super.makeOptions(config);
      IntParameter kP = new IntParameter(KMeans.K_ID, new GreaterConstraint(0));
      if(config.grab(kP)) {
        k = kP.getValue();
      }

      ObjectParameter<KMedoidsInitialization<V>> initialP = new ObjectParameter<KMedoidsInitialization<V>>(KMeans.INIT_ID, KMedoidsInitialization.class, PAMInitialMeans.class);
      if(config.grab(initialP)) {
        initializer = initialP.instantiateClass(config);
      }

      IntParameter maxiterP = new IntParameter(KMeans.MAXITER_ID, new GreaterEqualConstraint(0), 0);
      if(config.grab(maxiterP)) {
        maxiter = maxiterP.getValue();
      }
    }

    @Override
    protected KMedoidsEM<V, D> makeInstance() {
      return new KMedoidsEM<V, D>(distanceFunction, k, maxiter, initializer);
    }
  }
}
}
