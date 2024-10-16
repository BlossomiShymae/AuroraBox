using System.Reflection;
using AuroraBox;
using McMaster.Extensions.CommandLineUtils;

var app = new CommandLineApplication();
app.HelpOption();

var assembly = Assembly.GetExecutingAssembly();
app.ExtendedHelpText = $@"
{assembly.GetName().Name} {assembly.GetName().Version}
Generate static data from League Wiki's ARAM and Arena page.
";

await CommandLineApplication.ExecuteAsync<CommandLineInterface>(args);