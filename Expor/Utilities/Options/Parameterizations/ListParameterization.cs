using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Utilities.Options.Parameters;
using Socona.Expor.Utilities.Pairs;

namespace Socona.Expor.Utilities.Options.Parameterizations
{
  
public class ListParameterization : AbstractParameterization {
  /**
   * The actual parameters, for storage
   */
  List<Pair<OptionDescription, Object>> parameters = new List<Pair<OptionDescription, Object>>();

  /**
   * Default constructor.
   */
  public ListParameterization() :
    base(){
  }
  
  /**
   * Constructor with an existing collection.
   * 
   * @param dbParameters existing parameter collection
   */
  public ListParameterization(ICollection<Pair<OptionDescription, Object>> dbParameters) {
    
    foreach (Pair<OptionDescription, Object> pair in dbParameters) {
      AddParameter(pair.First, pair.Second);
    }
  }

  /**
   * Add a flag to the parameter list
   * 
   * @param optionid Option ID
   */
  public void AddFlag(OptionDescription optionid) {
    parameters.Add(new Pair<OptionDescription, Object>(optionid, BoolParameter.SET));
  }

  /**
   * Add a parameter to the parameter list
   * 
   * @param optionid Option ID
   * @param value Value
   */
  public void AddParameter(OptionDescription optionid, Object value) {
    parameters.Add(new Pair<OptionDescription, Object>(optionid, value));
  }
  
  /**
   * Convenience - Add a Flag option directly.
   * 
   * @param flag Flag to Add, if set
   */
  public void ForwardOption(BoolParameter flag) {
    if (flag.IsDefined() && flag.GetValue()) {
      AddFlag(flag.GetOptionDescription());
    }
  }
  
  /**
   * Convenience - Add a Parameter for forwarding
   * 
   * @param param Parameter to Add
   */
  public void ForwardOption(IParameter param) {
    if (param.IsDefined()) {
      AddParameter(param.GetOptionDescription(), param.GetValue());
    }
  }

  
  public override bool SetValueForOption(IParameter opt) { 
    //Iterator<Pair<OptionDescription, Object>> iter = parameters.iterator();
    //while(iter.hasNext()) {
      for(int i=0;i<parameters.Count;i++){
        
      Pair<OptionDescription, Object> pair = parameters[i];
      if(pair.First == opt.GetOptionDescription()) {
        parameters.Remove(pair);
        opt.SetValue(pair.Second);
        return true;
      }
    }
    return false;
  }

  /**
   * Return the yet unused parameters.
   * 
   * @return Unused parameters.
   */
  public IList<Pair<OptionDescription, Object>> GetRemainingParameters() {
    return parameters;
  }

  
  public override bool HasUnusedParameters() {
    return (parameters.Count> 0);
  }

  /** {@inheritDoc}
   * Default implementation, for flat parameterizations. 
   */
  
  public override IParameterization Descend(Object option) {
    return this;
  }

  
  public override String ToString() {
    StringBuilder buf = new StringBuilder();
    foreach (Pair<OptionDescription, Object> pair in parameters) {
      buf.Append("-").Append(pair.First.ToString()).Append(" ");
      buf.Append(pair.Second.ToString()).Append(" ");
    }
    return buf.ToString();
  }

  /**
   * Serialize parameters.
   * 
   * @return Array list of parameters
   */
  public IList<String> Serialize() {
    List<String> param = new List<String>();
    foreach (Pair<OptionDescription, Object> pair in parameters) {
      param.Add("-" + pair.GetFirst().ToString());
      param.Add(pair.GetSecond().ToString());
    }
    return param;
  }
}}
