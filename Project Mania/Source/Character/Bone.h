#pragma once

#include "Data/SkeletonData.h"

#include <Pargon.h>
#include <PargonScene.h>

using namespace Pargon;

class Bone
{
public:
	BoneData* Data;

	void CreateModel();
	void Initialize(BoneData& data);
	void Animate(float elapsed);

	auto GetGlobalTransform() const -> const Matrix4x4&;
	auto GetGlobalPosition() const -> Point3;
	auto GetGlobalRotation() const -> Quaternion;

	auto Position() const -> const Point3&;
	void SetPosition(Point3 position);

	auto Rotation() const -> const Quaternion&;
	void SetRotation(Quaternion rotation);

	auto Size() const -> Vector3;
	void SetSize(Vector3 size);

	auto Offset() const -> const Vector3&;
	void SetOffset(Vector3 offset);

	auto Children() -> SequenceView<std::unique_ptr<Bone>>;
	auto GetBone(StringView name) -> Bone*;

private:
	mutable bool _dirty = true;

	mutable Matrix4x4 _globalTransform;

	Point3 _position;
	Vector3 _size;
	Vector3 _offset;
	Quaternion _rotation = Quaternion::CreateIdentity();

	List<std::unique_ptr<Bone>> _children;

	Bone* _parent = nullptr;
	Model* _model;

	void SetDirty();
};

inline
auto Bone::Position() const -> const Point3&
{
	return _position;
}

inline
void Bone::SetPosition(Point3 position)
{
	_position = position;
	SetDirty();
}

inline
auto Bone::Rotation() const -> const Quaternion&
{
	return _rotation;
}

inline
void Bone::SetRotation(Quaternion rotation)
{
	_rotation = rotation;
	SetDirty();
}

inline
auto Bone::Size() const -> Vector3
{
	return _size;
}

inline
void Bone::SetSize(Vector3 size)
{
	_size = size;
	SetDirty();
}

inline
auto Bone::Offset() const -> const Vector3&
{
	return _offset;
}

inline
void Bone::SetOffset(Vector3 offset)
{
	_offset = offset;
	SetDirty();
}

inline
auto Bone::Children() -> SequenceView<std::unique_ptr<Bone>>
{
	return _children;
}
