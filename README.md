GenProj (c) 2014 Ian Norton (inorton@gmail.com)
------------------------------------------------

GenProj is a little tool that helps create/update basic csproj files. It is
intended for use in make files on windows or mono.

Examples
---------

$ buildcsystem --create --csproj foo.csproj --cs Program.cs --cs src/Guts.cs

Will create you a new file called foo.csproj that compiles Program.cs and
src/Guts.cs.

License
--------
This tool is free software; you can redistribute it and/or modify it under the
terms of the GNU Lesser General Public License as published by the Free
Software Foundation; either version 2.1 of the License, or (at your option) any
later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
details.

You should have received a copy of the GNU Lesser General Public License
along with this library; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
