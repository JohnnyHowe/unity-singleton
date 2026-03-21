using System;
using System.Collections.Generic;
using System.Linq;

namespace JonathonOH.Unity.Singletons
{
	internal class SingletonInitializationOrderSorter
	{
		private HashSet<ISingleton> _allSingletons;
		private HashSet<ISingleton> _notYetOrdered;
		private List<ISingleton> _ordered;
		Dictionary<Type, Type[]> _dependencies;

		internal SingletonInitializationOrderSorter(IEnumerable<ISingleton> singletons)
		{
			_allSingletons = new HashSet<ISingleton>(singletons);
		}

		internal IEnumerable<ISingleton> GetOrderedByDependencies()
		{
			_notYetOrdered = new HashSet<ISingleton>(_allSingletons);
			_ordered = new List<ISingleton>();
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
			ISingleton firstWithFulfilledDependencies = _notYetOrdered.FirstOrDefault((ISingleton singleton) => AreDependenciesSatisfied(singleton));

			if (firstWithFulfilledDependencies == null)
			{
				return false;
			}

			_notYetOrdered.Remove(firstWithFulfilledDependencies);
			_ordered.Add(firstWithFulfilledDependencies);

			return true;
		}

		private bool AreDependenciesSatisfied(ISingleton singleton)
		{
			return !GetDependenciesNotSatisfied(singleton).Any();
		}

		private IEnumerable<Type> GetDependenciesNotSatisfied(ISingleton singleton)
		{
			Dictionary<Type, bool> dependencySatisfaction = GetDependencySatisfaction(singleton);
			foreach (ISingleton realizedSystem in _ordered)
			{
				Type realizedType = realizedSystem.GetType();
				if (!dependencySatisfaction.ContainsKey(realizedType)) continue;
				dependencySatisfaction[realizedType] = true;
			}
			return dependencySatisfaction.Keys.Where((Type dependency) => !dependencySatisfaction[dependency]);
		}

		private Dictionary<Type, bool> GetDependencySatisfaction(ISingleton singleton)
		{
			Dictionary<Type, bool> dependencySatisfaction = new Dictionary<Type, bool>();
			foreach (Type type in _dependencies[singleton.GetType()])
			{
				dependencySatisfaction[type] = false;
			}
			return dependencySatisfaction;
		}

		private static Dictionary<Type, Type[]> GetDependencies(IEnumerable<ISingleton> singletons)
		{
			Dictionary<Type, Type[]> dependencies = new Dictionary<Type, Type[]>();
			foreach (ISingleton singleton in singletons)
			{
				dependencies[singleton.GetType()] = GetDependencies(singleton);
			}
			return dependencies;
		}

		private static Type[] GetDependencies(ISingleton singleton)
		{
			if (singleton is ISingletonDependencies singletonDependencies)
			{
				return singletonDependencies.RequiredSystems;
			}
			return new Type[0];
		}

	}
}
