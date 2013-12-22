#include <windows.h>
#include <stdio.h>
#include <io.h>
#include <fcntl.h>
#include "../Dependancies/detours.h"

#pragma comment(lib, "../Dependancies/detours.lib")

typedef BOOL(WINAPI* DCreateProcessWithTokenW)(
	HANDLE hToken,
	DWORD dwLogonFlags,
	LPCWSTR lpApplicationName,
	LPWSTR lpCommandLine,
	DWORD dwCreationFlags,
	LPVOID lpEnvironment,
	LPCWSTR lpCurrentDirectory,
	LPSTARTUPINFOW lpStartupInfo,
	LPPROCESS_INFORMATION lpProcessInformation
	);
typedef HMODULE (WINAPI* DLoadLibraryW)(LPCWSTR pFilename);
typedef void* (FAR *DMonoAssemblyOpen)(LPCSTR pFilename, int* pStatus);
typedef void* (FAR *DMonoAssemblyGetImage)(void* pAssembly);
typedef void* (FAR *DMonoClassFromName)(void* pImage, LPCSTR pNamespace, LPCSTR pName);
typedef void* (FAR *DMonoMethodDescNew)(LPCSTR pName, BOOL pIncludeNamespace);
typedef void* (FAR *DMonoMethodDescSearchInClass)(void* pDesc, void* pClass);
typedef void* (FAR *DMonoRuntimeInvoke)(void* pMethod, void* pObj, void** pParams, void** pExc);

DCreateProcessWithTokenW CreateProcessWithTokenWOriginal = CreateProcessWithTokenW;
DLoadLibraryW LoadLibraryWOriginal = LoadLibraryW;

DMonoAssemblyOpen MonoAssemblyOpenOriginal = NULL;
DMonoAssemblyGetImage MonoAssemblyGetImageOriginal = NULL;
DMonoClassFromName MonoClassFromNameOriginal = NULL;
DMonoMethodDescNew MonoMethodDescNewOriginal = NULL;
DMonoMethodDescSearchInClass MonoMethodDescSearchInClassOriginal = NULL;
DMonoRuntimeInvoke MonoRuntimeInvokeOriginal = NULL;

BOOL LoadLibraryWIsDetoured = FALSE;
BOOL CreateProcessWithTokenWIsDetoured = FALSE;
BOOL MonoRuntimeInvokeIsDetoured = FALSE;

BOOL InsideAgent = FALSE;

void* MonoRuntimeInvokeDetour(void* pMethod, void* pObj, void** pParams, void** pExc)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&MonoRuntimeInvokeOriginal, MonoRuntimeInvokeDetour);
	if (!DetourTransactionCommit())
	{
		MonoRuntimeInvokeIsDetoured = FALSE;

		BOOL loadingRuntime = strstr(GetCommandLineA(), "-session") != NULL;
		//if (!loadingRuntime)
		//{
		//	AllocConsole();
		//	*stdout = *_fdopen(_open_osfhandle((intptr_t)GetStdHandle(STD_OUTPUT_HANDLE), _O_TEXT), "w");
		//	setvbuf(stdout, NULL, _IONBF, 0);
		//}
		CHAR assemblyName[128];
		strcpy_s(assemblyName, loadingRuntime ? "StonehearthRuntime" : "StonehearthSniffer");

		CHAR assemblyPath[1024];
		HMODULE loaderHandle = GetModuleHandleA("StonehearthLoader.dll");
		GetModuleFileNameA(loaderHandle, assemblyPath, sizeof(assemblyPath));
		strrchr(assemblyPath, '\\')[1] = 0;
		strcat_s(assemblyPath, sizeof(assemblyPath), assemblyName);
		strcat_s(assemblyPath, sizeof(assemblyPath), ".dll");

		int status = -1;
		MonoRuntimeInvokeOriginal(
			MonoMethodDescSearchInClassOriginal(MonoMethodDescNewOriginal("Startup:Main", FALSE),
			MonoClassFromNameOriginal(MonoAssemblyGetImageOriginal(MonoAssemblyOpenOriginal(assemblyPath, &status)), assemblyName, "Startup")),
			NULL, NULL, NULL);
	}
	return MonoRuntimeInvokeOriginal(pMethod, pObj, pParams, pExc);
}

HMODULE WINAPI LoadLibraryWDetour(LPCWSTR pFilename)
{
	HMODULE handle = LoadLibraryWOriginal(pFilename);
	if (wcsstr(pFilename, L"mono.dll") != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach((PVOID*)&LoadLibraryWOriginal, LoadLibraryWDetour);
		if (!DetourTransactionCommit())
		{
			LoadLibraryWIsDetoured = FALSE;

			MonoAssemblyOpenOriginal = (DMonoAssemblyOpen)GetProcAddress(handle, "mono_assembly_open");
			MonoAssemblyGetImageOriginal = (DMonoAssemblyGetImage)GetProcAddress(handle, "mono_assembly_get_image");
			MonoClassFromNameOriginal = (DMonoClassFromName)GetProcAddress(handle, "mono_class_from_name");
			MonoMethodDescNewOriginal = (DMonoMethodDescNew)GetProcAddress(handle, "mono_method_desc_new");
			MonoMethodDescSearchInClassOriginal = (DMonoMethodDescSearchInClass)GetProcAddress(handle, "mono_method_desc_search_in_class");
			MonoRuntimeInvokeOriginal = (DMonoRuntimeInvoke)GetProcAddress(handle, "mono_runtime_invoke");

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&(PVOID&)MonoRuntimeInvokeOriginal, MonoRuntimeInvokeDetour);
			MonoRuntimeInvokeIsDetoured = !DetourTransactionCommit();
		}
	}
	return handle;
}

BOOL WINAPI CreateProcessWithTokenWDetour(
	HANDLE hToken,
	DWORD dwLogonFlags,
	LPCWSTR lpApplicationName,
	LPWSTR lpCommandLine,
	DWORD dwCreationFlags,
	LPVOID lpEnvironment,
	LPCWSTR lpCurrentDirectory,
	LPSTARTUPINFOW lpStartupInfo,
	LPPROCESS_INFORMATION lpProcessInformation
	)
{
	BOOL isHearthstone = wcsstr(lpApplicationName, L"Hearthstone") != NULL;
	if (!isHearthstone) return CreateProcessWithTokenWOriginal(hToken, dwLogonFlags, lpApplicationName, lpCommandLine, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
	BOOL result = CreateProcessW(lpApplicationName, lpCommandLine, NULL, NULL, FALSE, dwCreationFlags | CREATE_SUSPENDED, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);
	CHAR assemblyPath[1024];
	HMODULE loaderHandle = GetModuleHandleA("StonehearthLoader.dll");
	GetModuleFileNameA(loaderHandle, assemblyPath, sizeof(assemblyPath));
	size_t pathLength = strlen(assemblyPath) + 1;
	LPVOID pathRemote = VirtualAllocEx(lpProcessInformation->hProcess, NULL, pathLength, 0x1000, 0x40);
	SIZE_T written = 0;
	WriteProcessMemory(lpProcessInformation->hProcess, pathRemote, assemblyPath, pathLength, &written);
	FARPROC procLoadLibraryA = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA");
	DWORD threadID = 0;
	HANDLE hThread = CreateRemoteThread(lpProcessInformation->hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)procLoadLibraryA, pathRemote, 0, &threadID);
	WaitForSingleObject(hThread, -1);
	VirtualFreeEx(lpProcessInformation->hProcess, pathRemote, 0, 0x8000);
	CloseHandle(hThread);
	ResumeThread(lpProcessInformation->hThread);
	return result;
}

BOOL APIENTRY DllMain(__in HINSTANCE pInstance, __in DWORD pReason, __in __reserved LPVOID pReserved)
{
	UNREFERENCED_PARAMETER(pReserved);
	if (pReason == DLL_PROCESS_ATTACH)
	{
		InsideAgent = GetModuleHandleA("Agent.exe") != NULL;
		if (InsideAgent)
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&(PVOID&)CreateProcessWithTokenWOriginal, CreateProcessWithTokenWDetour);
			CreateProcessWithTokenWIsDetoured = !DetourTransactionCommit();
		}
		else
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&(PVOID&)LoadLibraryWOriginal, LoadLibraryWDetour);
			LoadLibraryWIsDetoured = !DetourTransactionCommit();
		}
	}
	else if (pReason == DLL_PROCESS_DETACH)
	{
		if (CreateProcessWithTokenWIsDetoured)
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourDetach((PVOID*)&CreateProcessWithTokenWOriginal, CreateProcessWithTokenWDetour);
			DetourTransactionCommit();
		}
		if (LoadLibraryWIsDetoured)
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourDetach((PVOID*)&LoadLibraryWOriginal, LoadLibraryWDetour);
			DetourTransactionCommit();
		}
		if (MonoRuntimeInvokeIsDetoured)
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourDetach((PVOID*)&MonoRuntimeInvokeOriginal, MonoRuntimeInvokeDetour);
			DetourTransactionCommit();
		}
	}
	return TRUE;
}