using System;
using System.Text;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Core.Operations;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros
{
    // Symbol.type = "computed" is the only thing that becomes an evaluate macro.
    public class EvaluateMacro : IMacro
    {
        public Guid Id => new Guid("BB625F71-6404-4550-98AF-B2E546F46C5F");

        public string Type => "evaluate";

        public static readonly string DefaultEvaluator = "C++";

        public void EvaluateConfig(IEngineEnvironmentSettings environmentSettings, IVariableCollection vars, IMacroConfig rawConfig, IParameterSet parameters, ParameterSetter setter)
        {
            EvaluateMacroConfig config = rawConfig as EvaluateMacroConfig;

            if (config == null)
            {
                throw new InvalidCastException("Couldn't cast the rawConfig as EvaluateMacroConfig");
            }

            ConditionEvaluator evaluator = EvaluatorSelector.Select(config.Evaluator);

            byte[] data = Encoding.UTF8.GetBytes(config.Value);
            int len = data.Length;
            int pos = 0;
            IProcessorState state = new GlobalRunSpec.ProcessorState(environmentSettings, vars, data, Encoding.UTF8);
            bool result = evaluator(state, ref len, ref pos, out bool faulted);

            Parameter p = new Parameter
            {
                IsVariable = true,
                Name = config.VariableName
            };

            setter(p, result.ToString());
        }
    }
}