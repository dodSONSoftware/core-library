﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the <see cref="UnInstallPackage"/> request.
    /// </summary>
    [Serializable]
    public class UnInstallPackageResponse
    {
        #region Ctor
        private UnInstallPackageResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="UnInstallPackageResponse"/> with the given result and reason.
        /// </summary>
        /// <param name="results">Indicates whether the un-installation process was successful or not. <b>True</b> indicates success; otherwise, <b>false</b> indicates a failure.</param>
        /// <param name="reason">The reason for the un-installation success or failure.</param>
        /// <param name="packageName">The name of the package being uninstalled.</param>
        /// <param name="packageVersion">The version of the package being uninstalled.</param>
        public UnInstallPackageResponse(bool results,
                                        string reason,
                                        string packageName,
                                        Version packageVersion)
            : this()
        {
            Results = results;
            //
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException(nameof(reason));
            }
            Reason = reason;
            //
            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException(nameof(packageName));
            }
            PackageName = packageName;
            //
            PackageVersion = packageVersion ?? throw new ArgumentNullException(nameof(packageVersion));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Indicates whether the un-installation process was successful or not. <b>True</b> indicates success; otherwise, <b>false</b> indicates a failure.
        /// </summary>
        public bool Results
        {
            get;
        }
        /// <summary>
        /// The reason for the un-installation success or failure.
        /// </summary>
        public string Reason
        {
            get;
        }
        /// <summary>
        /// The name of the package being uninstalled.
        /// </summary>
        public string PackageName
        {
            get;
        }
        /// <summary>
        /// The version of the package being uninstalled.
        /// </summary>
        public Version PackageVersion
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Results={Results}, Reason={Reason}, Package Name={PackageName}, Package Version={PackageVersion}";
        #endregion
    }
}
