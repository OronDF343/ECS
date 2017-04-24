using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS.Model
{
    public class CircuitState
    {
        public string Name { get; set; }
        public Dictionary<Guid, bool> SwitchStates { get; set; } = new Dictionary<Guid, bool>();

        public static string Serialize(IEnumerable<CircuitState> states)
        {
            var statesL = states.ToList();
            var usedIds = statesL.SelectMany(s => s.SwitchStates.Keys).Distinct().ToList();
            var result = "[name]," + string.Join(",", usedIds) + "\n";
            var lines = statesL.Select(s => s.Name + ","
                                            + string.Join(",", usedIds.Select(i => !s.SwitchStates.ContainsKey(i)
                                                                                       ? "*"
                                                                                       : s.SwitchStates[i]
                                                                                           ? "1"
                                                                                           : "0")));
            result += string.Join("\n", lines);
            return result;
        }

        public static IEnumerable<CircuitState> Deserialize(string csv, List<Guid> usedIds = null)
        {
            var lines = csv.Split('\n');
            if (usedIds == null) usedIds = lines[0].Split(',').Skip(1).Select(Guid.Parse).ToList();
            return from line in lines.Skip(1)
                   let st = line.Split(',')
                   select new CircuitState
                   {
                       Name = st[0],
                       SwitchStates = usedIds
                           .Select((i, n) => st[n + 1] == "*" ? null : new { Id = i, State = st[n + 1] == "1" })
                           .Where(g => g != null)
                           .ToDictionary(g => g.Id, g => g.State)
                   };
        }
    }
}
