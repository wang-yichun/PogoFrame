using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PrefabEvolution
{
	[System.Serializable]
	public class PELinkage
	{
		[System.Serializable]
		public class Link
		{
			public int LIIF;
			public Object InstanceTarget;

			public override string ToString()
			{
				return string.Format("[Link]{0}:{1}", LIIF, InstanceTarget);
			}
		}

		public List<Link> Links = Utils.Create<List<Link>>();

		public Link this[int liif]
		{
			get
			{
				return Links.FirstOrDefault(link => link.LIIF == liif);
			}
		}

		public Link this[Link link]
		{
			get
			{
				if (link == null)
					return null;

				return this[link.LIIF];
			}
		}

		public Link this[Object obj]
		{
			get
			{
				return Links.FirstOrDefault(link => link.InstanceTarget == obj);
			}
		}

		public Object GetPrefabObject(GameObject prefab, Object instanceObject)
		{
			var prafabPI = prefab.GetComponent<PEPrefabScript>();
			var lnk = this[instanceObject];

			if (lnk == null)
				return null;

			var link = prafabPI.Links[lnk.LIIF];
			if (link == null)
				return null;

			return link.InstanceTarget;
		}

	}

}