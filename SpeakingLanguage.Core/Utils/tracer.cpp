#pragma once
#include "stdafx.h"
#include "tracer.h"

using namespace SpeakingLanguage::Utils;

void
tracer::Log(const std::string& msg)
{
	std::cout << "[sllog] " << msg << std::endl;
}

void
tracer::Log(std::string&& msg)
{
	std::cout << "[sllog] " << msg << std::endl;
}