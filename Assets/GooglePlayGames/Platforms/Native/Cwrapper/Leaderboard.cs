/*
 * Copyright (C) 2014 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#if (UNITY_ANDROID )
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GooglePlayGames.Native.Cwrapper {
internal static class Leaderboard {
    [DllImport(SymbolLocation.NativeSymbolLocation)]
    internal static extern /* from(size_t) */ UIntPtr Leaderboard_Name(
        HandleRef self,
         /* from(char *) */ StringBuilder out_arg,
         /* from(size_t) */ UIntPtr out_size);

    [DllImport(SymbolLocation.NativeSymbolLocation)]
    internal static extern /* from(size_t) */ UIntPtr Leaderboard_Id(
        HandleRef self,
         /* from(char *) */ StringBuilder out_arg,
         /* from(size_t) */ UIntPtr out_size);

    [DllImport(SymbolLocation.NativeSymbolLocation)]
    internal static extern /* from(size_t) */ UIntPtr Leaderboard_IconUrl(
        HandleRef self,
         /* from(char *) */ StringBuilder out_arg,
         /* from(size_t) */ UIntPtr out_size);

    [DllImport(SymbolLocation.NativeSymbolLocation)]
    internal static extern void Leaderboard_Dispose(
        HandleRef self);

    [DllImport(SymbolLocation.NativeSymbolLocation)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern /* from(bool) */ bool Leaderboard_Valid(
        HandleRef self);

    [DllImport(SymbolLocation.NativeSymbolLocation)]
    internal static extern /* from(LeaderboardOrder_t) */ Types.LeaderboardOrder Leaderboard_Order(
        HandleRef self);
}
}
#endif // (UNITY_ANDROID || UNITY_IPHONE)
