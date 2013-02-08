// <copyright file="SharedOptionsBase.cs" company="Katana contributors">
//   Copyright 2011-2013 Katana contributors
// </copyright>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Owin.StaticFiles.FileSystems;

namespace Microsoft.Owin.StaticFiles.Infrastructure
{
    /// <summary>
    /// Options common to several middleware components
    /// </summary>
    /// <typeparam name="T">The type of the subclass</typeparam>
    public abstract class SharedOptionsBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedOptions"></param>
        protected SharedOptionsBase(SharedOptions sharedOptions)
        {
            SharedOptions = sharedOptions;
        }

        /// <summary>
        /// Options common to several middleware components
        /// </summary>
        protected SharedOptions SharedOptions { get; private set; }

        /// <summary>
        /// The request path that maps to static resources
        /// </summary>
        public string RequestPath
        {
            get { return SharedOptions.RequestPath; }
            set { SharedOptions.RequestPath = value; }
        }

        /// <summary>
        /// The file system used to locate resources
        /// </summary>
        public IFileSystemProvider FileSystemProvider
        {
            get { return SharedOptions.FileSystemProvider; }
            set { SharedOptions.FileSystemProvider = value; }
        }

        /// <summary>
        /// Sets the request path
        /// </summary>
        /// <param name="path">The request path</param>
        /// <returns>this</returns>
        public T WithRequestPath(string path)
        {
            RequestPath = path;
            return (T)(object)this;
        }

        /// <summary>
        /// Sets the file system
        /// </summary>
        /// <param name="fileSystemProvider">The file system</param>
        /// <returns>this</returns>
        public T WithFileSystemProvider(IFileSystemProvider fileSystemProvider)
        {
            FileSystemProvider = fileSystemProvider;
            return (T)(object)this;
        }

        /// <summary>
        /// Sets a physical file system at the given disk path
        /// </summary>
        /// <param name="path">The root disk path</param>
        /// <returns>this</returns>
        public T WithPhysicalPath(string path)
        {
            return WithFileSystemProvider(new PhysicalFileSystemProvider(path));
        }
    }
}