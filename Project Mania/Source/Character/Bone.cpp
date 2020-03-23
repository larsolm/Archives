#include "Pch.h"
#include "Character/Bone.h"
#include "Game/Game.h"

void Bone::Initialize(BoneData& data)
{
	Data = &data;

	_position = data.Position;
	_size = data.Size;
	_offset = data.Offset;
	_rotation = data.Rotation;

	for (auto& child : data.Children)
	{
		auto& bone = _children.Increment(std::make_unique<Bone>());
		bone->_parent = this;
		bone->Initialize(child);
	}
}

void Bone::CreateModel()
{
	_model = Game()->Rendering.CreateModel();

	for (auto& child : _children)
		child->CreateModel();
}

void Bone::Animate(float elapsed)
{
	_model->Transform = GetGlobalTransform();

	for (auto& child : _children)
		child->Animate(elapsed);
}

auto Bone::GetGlobalTransform() const -> const Matrix4x4&
{
	if (_dirty)
	{
		if (_parent == nullptr)
		{
			_globalTransform = Matrix4x4::CreateTransform({ _position.X, _position.Y, _position.Z }, _size, _rotation, _offset);
		}
		else
		{
			_globalTransform = Matrix4x4::CreateTranslation(-_offset);
			_globalTransform.Scale(_size);
			_globalTransform.Rotate(_rotation);
			_globalTransform.Scale({ 1.0f / _parent->_size.X, 1.0f / _parent->_size.Y, 1.0f / _parent->_size.Z });
			_globalTransform.Translate({ _position.X, _position.Y, _position.Z });
			_globalTransform *= _parent->GetGlobalTransform();
		}

		_dirty = false;
	}

	return _globalTransform;
}

auto Bone::GetGlobalPosition() const -> Point3
{
	auto& transform = GetGlobalTransform();
	return Point3{ _offset.X, _offset.Y, _offset.Z } * transform;
}

auto Bone::GetGlobalRotation() const -> Quaternion
{
	if (_parent != nullptr)
		return _rotation * _parent->GetGlobalRotation();

	return _rotation;
}

void Bone::SetDirty()
{
	_dirty = true;

	for (auto& child : _children)
		child->SetDirty();
}

auto Bone::GetBone(StringView name) -> Bone*
{
	for (auto& child : _children)
	{
		if (child->Data->Name == name)
			return child.get();

		auto found = child->GetBone(name);
		if (found != nullptr)
			return found;
	}

	return nullptr;
}
