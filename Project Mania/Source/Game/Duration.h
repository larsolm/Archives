#pragma once

#include <Pargon.h>

using namespace Pargon;

class Duration
{
public:
	static constexpr auto Infinite() -> Duration;
	static constexpr auto FromTicks(unsigned int ticks) -> Duration;
	static constexpr auto FromSeconds(float seconds, unsigned int tickRate) -> Duration;
	static constexpr auto FromMinutes(float minutes, unsigned int tickRate) -> Duration;
	static constexpr auto FromHertz(float rate, unsigned int tickRate) -> Duration;

	constexpr auto IsInfinite() const -> bool;
	constexpr auto Ticks() const -> unsigned int;
	constexpr auto InSeconds(unsigned int tickRate) const -> float;
	constexpr auto InMinutes(unsigned int tickRate) const -> float;
	constexpr auto InHertz(unsigned int tickRate) const -> float;

	constexpr auto operator==(const Duration& right) const -> bool;
	constexpr auto operator!=(const Duration& right) const -> bool;
	constexpr auto operator<(const Duration& right) const -> bool;
	constexpr auto operator>(const Duration& right) const -> bool;
	constexpr auto operator<=(const Duration& right) const -> bool;
	constexpr auto operator>=(const Duration& right) const -> bool;

	constexpr auto operator-(const Duration& right) const -> Duration;
	constexpr auto operator+(const Duration& right) const -> Duration;
	constexpr auto operator-=(const Duration& right) -> Duration&;
	constexpr auto operator+=(const Duration& right) -> Duration&;
	constexpr auto operator--() -> Duration&;
	constexpr auto operator++() -> Duration&;
	constexpr auto operator--(int) -> Duration;
	constexpr auto operator++(int) -> Duration;

	constexpr auto operator*(float scalar) const -> Duration;
	constexpr auto operator/(float scalar) const -> Duration;
	constexpr auto operator*=(float scalar) -> Duration&;
	constexpr auto operator/=(float scalar) -> Duration&;

	constexpr auto operator*(int scalar) const -> Duration;
	constexpr auto operator/(int scalar) const -> Duration;
	constexpr auto operator*=(int scalar) -> Duration&;
	constexpr auto operator/=(int scalar) -> Duration&;

	auto FromBuffer(BufferReader& reader) -> bool;
	void ToBuffer(BufferWriter& writer) const;

	auto FromBlueprint(BlueprintReader& reader) -> bool;
	void ToBlueprint(BlueprintWriter& writer) const;

	auto FromString(StringReader& reader) -> bool;
	void ToString(StringWriter& writer) const;

private:
	static constexpr auto Clamp(float seconds) -> unsigned int;

	constexpr Duration(unsigned int ticks);

	unsigned int _ticks;

};

constexpr auto operator""_ticks(unsigned long long ticks) -> Duration;

constexpr
Duration::Duration(unsigned int ticks) :
	_ticks(ticks)
{
}

constexpr
auto Duration::Infinite() -> Duration
{
	return { std::numeric_limits<unsigned int>::max() };
}

constexpr
auto Duration::FromTicks(unsigned int ticks) -> Duration
{
	return { ticks };
}

constexpr
auto Duration::FromSeconds(float seconds, unsigned int tickRate) -> Duration
{
	return Duration(Clamp(seconds * tickRate));
}

constexpr
auto Duration::FromMinutes(float minutes, unsigned int tickRate) -> Duration
{
	return Duration(Clamp((minutes * 60) * tickRate));
}

constexpr
auto Duration::FromHertz(float rate, unsigned int tickRate) -> Duration
{
	return FromSeconds(1.0f / rate, tickRate);
}

constexpr
auto Duration::IsInfinite() const -> bool
{
	return _ticks == std::numeric_limits<unsigned int>::max();
}

constexpr
auto Duration::Ticks() const -> unsigned int
{
	return _ticks;
}

constexpr
auto Duration::InSeconds(unsigned int tickRate) const -> float
{
	return _ticks / static_cast<float>(tickRate);
}

constexpr
auto Duration::InMinutes(unsigned int tickRate) const -> float
{
	return (_ticks / static_cast<float>(tickRate)) / 60.0f;
}

constexpr
auto Duration::InHertz(unsigned int tickRate) const -> float
{
	return 1.0f / InSeconds(tickRate);
}

constexpr
auto Duration::Clamp(float seconds) -> unsigned int
{
	return static_cast<unsigned int>(seconds + 0.5f);
}

constexpr
auto Duration::operator==(const Duration& right) const -> bool
{
	return _ticks == right._ticks;
}

constexpr
auto Duration::operator!=(const Duration& right) const -> bool
{
	return _ticks != right._ticks;
}

constexpr
auto Duration::operator<(const Duration& right) const -> bool
{
	return _ticks < right._ticks;
}

constexpr
auto Duration::operator>(const Duration& right) const -> bool
{
	return _ticks > right._ticks;
}

constexpr
auto Duration::operator<=(const Duration& right) const -> bool
{
	return _ticks <= right._ticks;
}

constexpr
auto Duration::operator>=(const Duration& right) const -> bool
{
	return _ticks >= right._ticks;
}

constexpr
auto Duration::operator-(const Duration& right) const -> Duration
{
	return Duration::FromTicks(_ticks - right._ticks);
}

constexpr
auto Duration::operator+(const Duration& right) const -> Duration
{
	return Duration::FromTicks(_ticks + right._ticks);
}

constexpr
auto Duration::operator-=(const Duration& right) -> Duration&
{
	_ticks -= right._ticks;
	return *this;
}

constexpr
auto Duration::operator+=(const Duration& right) -> Duration&
{
	_ticks += right._ticks;
	return *this;
}

constexpr
auto Duration::operator--() -> Duration&
{
	_ticks--;
	return *this;
}

constexpr
auto Duration::operator++() -> Duration&
{
	_ticks++;
	return *this;
}

constexpr
auto Duration::operator--(int) -> Duration
{
	auto temp = *this;
	--*this;
	return temp;
}

constexpr
auto Duration::operator++(int) -> Duration
{
	auto temp = *this;
	++*this;
	return temp;
}

constexpr
auto Duration::operator*(float scalar) const -> Duration
{
	return Duration::FromTicks(Clamp(_ticks * scalar));
}

constexpr
auto Duration::operator/(float scalar) const -> Duration
{
	return Duration::FromTicks(Clamp(_ticks / scalar));
}

constexpr
auto Duration::operator*=(float scalar) -> Duration&
{
	_ticks = Clamp(_ticks * scalar);
	return *this;
}

constexpr
auto Duration::operator/=(float scalar) -> Duration&
{
	_ticks = Clamp(_ticks / scalar);
	return *this;
}

constexpr
auto Duration::operator*(int scalar) const -> Duration
{
	return Duration::FromTicks(_ticks * scalar);
}

constexpr
auto Duration::operator/(int scalar) const -> Duration
{
	return Duration::FromTicks(_ticks / scalar);
}

constexpr
auto Duration::operator*=(int scalar) -> Duration&
{
	_ticks *= scalar;
	return *this;
}

constexpr
auto Duration::operator/=(int scalar) -> Duration&
{
	_ticks /= scalar;
	return *this;
}

constexpr
auto operator""_ticks(unsigned long long ticks) -> Duration
{
	return Duration::FromTicks(static_cast<int>(ticks));
}
