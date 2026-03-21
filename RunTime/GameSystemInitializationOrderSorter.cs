using System;
using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.UnityTools.SystemsManagement
{
	internal class GameSystemInitializationOrderSorter
	{
		private HashSet<IGameSystem> _allGameSystems;
		private HashSet<IGameSystem> _notYetOrdered;
		private List<IGameSystem> _ordered;
		Dictionary<Type, Type[]> _dependencies;

		public GameSystemInitializationOrderSorter(IEnumerable<IGameSystem> gameSystems)
		{
			_allGameSystems = new HashSet<IGameSystem>(gameSystems);
		}

		public IEnumerable<IGameSystem> GetOrderedByDependencies()
		{
			_notYetOrdered = new HashSet<IGameSystem>(_allGameSystems);
			_ordered = new List<IGameSystem>();
			_dependencies = GetDependencies(_notYetOrdered);

			while (RunSortIteration()) { }

			if (_notYetOrdered.Count > 0)
			{
				throw new Exception($"Could not get initialization order for dependencies {string.Join(", ", _notYetOrdered)}.\nAre there circular or invalid dependencies?");
			}

			return _ordered;
		}

		/// <summary>
		/// Returns true if any item was added to _ordered.
		/// </summary>
		private bool RunSortIteration()
		{
			IGameSystem firstWithFulfilledDependencies = _notYetOrdered.FirstOrDefault((IGameSystem gameSystem) => AreDependenciesSatisfied(gameSystem));

			if (firstWithFulfilledDependencies == null)
			{
				return false;
			}

			_notYetOrdered.Remove(firstWithFulfilledDependencies);
			_ordered.Add(firstWithFulfilledDependencies);

			return true;
		}

		private bool AreDependenciesSatisfied(IGameSystem gameSystem)
		{
			return !GetDependenciesNotSatisfied(gameSystem).Any();
		}

		private IEnumerable<Type> GetDependenciesNotSatisfied(IGameSystem gameSystem)
		{
			Dictionary<Type, bool> dependencySatisfaction = GetDependencySatisfaction(gameSystem);
			foreach (IGameSystem realizedSystem in _ordered)
			{
				Type realizedType = realizedSystem.GetType();
				if (!dependencySatisfaction.ContainsKey(realizedType)) continue;
				dependencySatisfaction[realizedType] = true;
			}
			return dependencySatisfaction.Keys.Where((Type dependency) => !dependencySatisfaction[dependency]);
		}

		private Dictionary<Type, bool> GetDependencySatisfaction(IGameSystem gameSystem)
		{
			Dictionary<Type, bool> dependencySatisfaction = new Dictionary<Type, bool>();
			foreach (Type type in _dependencies[gameSystem.GetType()])
			{
				dependencySatisfaction[type] = false;
			}
			return dependencySatisfaction;
		}

		private static Dictionary<Type, Type[]> GetDependencies(IEnumerable<IGameSystem> gameSystems)
		{
			Dictionary<Type, Type[]> dependencies = new Dictionary<Type, Type[]>();
			foreach (IGameSystem gameSystem in gameSystems)
			{
				dependencies[gameSystem.GetType()] = GetDependencies(gameSystem);
			}
			return dependencies;
		}

		private static Type[] GetDependencies(IGameSystem gameSystem)
		{
			if (gameSystem is IGameSystemDependencies gameSystemDependencies)
			{
				return gameSystemDependencies.RequiredSystems;
			}
			return new Type[0];
		}

	}
}
